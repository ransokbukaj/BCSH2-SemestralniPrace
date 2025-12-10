-- 1. POMOCNÁ FUNKCE PRO ZÍSKÁNÍ DATOVÉHO TYPU SLOUPCE
CREATE OR REPLACE FUNCTION f_get_column_datatype(
    p_data_type     IN VARCHAR2,
    p_data_length   IN NUMBER,
    p_data_precision IN NUMBER,
    p_data_scale    IN NUMBER
) RETURN VARCHAR2
IS
    v_result VARCHAR2(100);
BEGIN
    CASE p_data_type
        WHEN 'VARCHAR2' THEN
            v_result := 'VARCHAR2(' || p_data_length || ')';
        WHEN 'NUMBER' THEN
            IF p_data_precision IS NOT NULL THEN
                IF p_data_scale IS NOT NULL AND p_data_scale > 0 THEN
                    v_result := 'NUMBER(' || p_data_precision || ',' || p_data_scale || ')';
                ELSE
                    v_result := 'NUMBER(' || p_data_precision || ')';
                END IF;
            ELSE
                v_result := 'NUMBER';
            END IF;
        WHEN 'DATE' THEN
            v_result := 'DATE';
        WHEN 'CLOB' THEN
            v_result := 'CLOB';
        WHEN 'BLOB' THEN
            v_result := 'BLOB';
        WHEN 'CHAR' THEN
            v_result := 'CHAR(' || p_data_length || ')';
        ELSE
            v_result := p_data_type;
    END CASE;
    
    RETURN v_result;
END;
/

-- 2. FUNKCE PRO ZÍSKÁNÍ INFORMACÍ O TABULKÁCH
CREATE OR REPLACE FUNCTION f_get_tables_info
RETURN SYS_REFCURSOR
IS
    v_cursor SYS_REFCURSOR;
BEGIN
    OPEN v_cursor FOR
        SELECT 
            t.table_name AS nazev_tabulky,
            tc.comments AS popis,
            (SELECT COUNT(*) 
             FROM all_tab_columns c 
             WHERE c.table_name = t.table_name 
             AND c.owner = t.owner) AS pocet_sloupcu,
            (SELECT COUNT(*) 
             FROM all_constraints con 
             WHERE con.table_name = t.table_name 
             AND con.owner = t.owner 
             AND con.constraint_type = 'R') AS pocet_cizich_klicu,
            t.num_rows AS odhadovany_pocet_radku,
            ROUND(t.blocks * 8192 / 1024 / 1024, 2) AS velikost_mb
        FROM all_tables t
        LEFT JOIN all_tab_comments tc 
            ON t.table_name = tc.table_name 
            AND t.owner = tc.owner
        WHERE t.owner = USER
        AND t.table_name NOT LIKE 'BIN$%'
        ORDER BY t.table_name;
    
    RETURN v_cursor;
END;
/

-- 3. FUNKCE PRO ZÍSKÁNÍ INFORMACÍ O SLOUPCÍCH TABULKY
CREATE OR REPLACE FUNCTION f_get_columns_info(
    p_table_name IN VARCHAR2
)
RETURN SYS_REFCURSOR
IS
    v_cursor SYS_REFCURSOR;
BEGIN
    OPEN v_cursor FOR
        SELECT 
            c.column_id AS poradi,
            c.column_name AS nazev_sloupce,
            f_get_column_datatype(
                c.data_type, 
                c.data_length, 
                c.data_precision, 
                c.data_scale
            ) AS datovy_typ,
            CASE 
                WHEN c.nullable = 'N' THEN 'NE'
                ELSE 'ANO'
            END AS nullable,
            c.data_default AS vychozi_hodnota,
            cc.comments AS popis,
            CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM all_cons_columns acc
                    JOIN all_constraints ac 
                        ON acc.constraint_name = ac.constraint_name 
                        AND acc.owner = ac.owner
                    WHERE ac.table_name = p_table_name
                    AND ac.owner = USER
                    AND ac.constraint_type = 'P'
                    AND acc.column_name = c.column_name
                ) THEN 'ANO'
                ELSE 'NE'
            END AS je_primarni_klic,
            CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM all_cons_columns acc
                    JOIN all_constraints ac 
                        ON acc.constraint_name = ac.constraint_name 
                        AND acc.owner = ac.owner
                    WHERE ac.table_name = p_table_name
                    AND ac.owner = USER
                    AND ac.constraint_type = 'R'
                    AND acc.column_name = c.column_name
                ) THEN 'ANO'
                ELSE 'NE'
            END AS je_cizi_klic
        FROM all_tab_columns c
        LEFT JOIN all_col_comments cc 
            ON c.table_name = cc.table_name 
            AND c.column_name = cc.column_name
            AND c.owner = cc.owner
        WHERE c.table_name = p_table_name
        AND c.owner = USER
        ORDER BY c.column_id;
    
    RETURN v_cursor;
END;
/

-- 4. FUNKCE PRO ZÍSKÁNÍ INFORMACÍ O PRIMÁRNÍCH KLÍČÍCH
CREATE OR REPLACE FUNCTION f_get_primary_keys
RETURN SYS_REFCURSOR
IS
    v_cursor SYS_REFCURSOR;
BEGIN
    OPEN v_cursor FOR
        SELECT 
            ac.table_name AS nazev_tabulky,
            ac.constraint_name AS nazev_omezeni,
            LISTAGG(acc.column_name, ', ') 
                WITHIN GROUP (ORDER BY acc.position) AS sloupce
        FROM all_constraints ac
        JOIN all_cons_columns acc 
            ON ac.constraint_name = acc.constraint_name 
            AND ac.owner = acc.owner
        WHERE ac.owner = USER
        AND ac.constraint_type = 'P'
        AND ac.table_name NOT LIKE 'BIN$%'  -- Vyfiltrování recycle bin
        GROUP BY ac.table_name, ac.constraint_name
        ORDER BY ac.table_name;
    
    RETURN v_cursor;
END;
/

-- 5. FUNKCE PRO ZÍSKÁNÍ INFORMACÍ O CIZÍCH KLÍČÍCH
CREATE OR REPLACE FUNCTION f_get_foreign_keys
RETURN SYS_REFCURSOR
IS
    v_cursor SYS_REFCURSOR;
BEGIN
    OPEN v_cursor FOR
        SELECT 
            ac.table_name AS tabulka_zdroj,
            ac.constraint_name AS nazev_omezeni,
            LISTAGG(acc.column_name, ', ') 
                WITHIN GROUP (ORDER BY acc.position) AS sloupce_zdroj,
            ac_pk.table_name AS tabulka_cil,
            LISTAGG(acc_pk.column_name, ', ') 
                WITHIN GROUP (ORDER BY acc_pk.position) AS sloupce_cil,
            CASE ac.delete_rule
                WHEN 'CASCADE' THEN 'CASCADE'
                WHEN 'SET NULL' THEN 'SET NULL'
                ELSE 'NO ACTION'
            END AS pravidlo_mazani
        FROM all_constraints ac
        JOIN all_cons_columns acc 
            ON ac.constraint_name = acc.constraint_name 
            AND ac.owner = acc.owner
        JOIN all_constraints ac_pk 
            ON ac.r_constraint_name = ac_pk.constraint_name 
            AND ac.r_owner = ac_pk.owner
        JOIN all_cons_columns acc_pk 
            ON ac_pk.constraint_name = acc_pk.constraint_name 
            AND ac_pk.owner = acc_pk.owner
        WHERE ac.owner = USER
        AND ac.constraint_type = 'R'
        AND ac.table_name NOT LIKE 'BIN$%'  -- Vyfiltrování recycle bin
        AND ac_pk.table_name NOT LIKE 'BIN$%'  -- Vyfiltrování recycle bin
        GROUP BY 
            ac.table_name, 
            ac.constraint_name, 
            ac_pk.table_name, 
            ac.delete_rule
        ORDER BY ac.table_name, ac.constraint_name;
    
    RETURN v_cursor;
END;
/

-- 6. FUNKCE PRO ZÍSKÁNÍ INFORMACÍ O INDEXECH
CREATE OR REPLACE FUNCTION f_get_indexes_info
RETURN SYS_REFCURSOR
IS
    v_cursor SYS_REFCURSOR;
BEGIN
    OPEN v_cursor FOR
        SELECT 
            ai.table_name AS nazev_tabulky,
            ai.index_name AS nazev_indexu,
            CASE ai.uniqueness
                WHEN 'UNIQUE' THEN 'UNIKÁTNÍ'
                ELSE 'NEUNIKÁTNÍ'
            END AS typ_indexu,
            LISTAGG(aic.column_name, ', ') 
                WITHIN GROUP (ORDER BY aic.column_position) AS sloupce,
            ai.num_rows AS pocet_radku,
            ai.distinct_keys AS pocet_unikaltnich_hodnot,
            CASE ai.status
                WHEN 'VALID' THEN 'PLATNÝ'
                ELSE 'NEPLATNÝ'
            END AS stav
        FROM all_indexes ai
        JOIN all_ind_columns aic 
            ON ai.index_name = aic.index_name 
            AND ai.owner = aic.index_owner
        WHERE ai.owner = USER
        AND ai.table_name NOT LIKE 'BIN$%'
        GROUP BY 
            ai.table_name,
            ai.index_name,
            ai.uniqueness,
            ai.num_rows,
            ai.distinct_keys,
            ai.status
        ORDER BY ai.table_name, ai.index_name;
    
    RETURN v_cursor;
END;
/

CREATE OR REPLACE FUNCTION f_get_views_info
RETURN SYS_REFCURSOR
IS
    v_cursor SYS_REFCURSOR;
BEGIN
    OPEN v_cursor FOR
        SELECT 
            v.view_name AS nazev_pohledu,
            tc.comments AS popis,
            (SELECT COUNT(*) 
             FROM user_tab_columns c 
             WHERE c.table_name = v.view_name) AS pocet_sloupcu,
            NULL AS definice
        FROM user_views v
        LEFT JOIN user_tab_comments tc 
            ON v.view_name = tc.table_name
        ORDER BY v.view_name;
    
    RETURN v_cursor;
END;
/

-- 8. FUNKCE PRO ZÍSKÁNÍ INFORMACÍ O SEKVENCÍCH
CREATE OR REPLACE FUNCTION f_get_sequences_info
RETURN SYS_REFCURSOR
IS
    v_cursor SYS_REFCURSOR;
BEGIN
    OPEN v_cursor FOR
        SELECT 
            sequence_name AS nazev_sekvence,
            min_value AS minimalni_hodnota,
            max_value AS maximalni_hodnota,
            increment_by AS krok,
            last_number AS posledni_hodnota,
            CASE 
                WHEN cache_size = 0 THEN 'NE'
                ELSE 'ANO (' || TO_CHAR(cache_size) || ')'
            END AS cache,
            CASE cycle_flag
                WHEN 'Y' THEN 'ANO'
                ELSE 'NE'
            END AS cyklicky,
            CASE order_flag
                WHEN 'Y' THEN 'ANO'
                ELSE 'NE'
            END AS serazeny
        FROM user_sequences
        ORDER BY sequence_name;
    
    RETURN v_cursor;
END;
/

-- 9. FUNKCE PRO ZÍSKÁNÍ INFORMACÍ O TRIGGERECH
CREATE OR REPLACE FUNCTION f_get_triggers_info
RETURN SYS_REFCURSOR
IS
    v_cursor SYS_REFCURSOR;
BEGIN
    OPEN v_cursor FOR
        SELECT 
            trigger_name AS nazev_triggeru,
            table_name AS nazev_tabulky,
            CASE triggering_event
                WHEN 'INSERT' THEN 'INSERT'
                WHEN 'UPDATE' THEN 'UPDATE'
                WHEN 'DELETE' THEN 'DELETE'
                ELSE triggering_event
            END AS udalost,
            CASE trigger_type
                WHEN 'BEFORE EACH ROW' THEN 'BEFORE EACH ROW'
                WHEN 'AFTER EACH ROW' THEN 'AFTER EACH ROW'
                WHEN 'BEFORE STATEMENT' THEN 'BEFORE STATEMENT'
                WHEN 'AFTER STATEMENT' THEN 'AFTER STATEMENT'
                ELSE trigger_type
            END AS typ,
            CASE status
                WHEN 'ENABLED' THEN 'AKTIVNÍ'
                ELSE 'NEAKTIVNÍ'
            END AS stav,
            SUBSTR(description, 1, 100) AS popis
        FROM all_triggers
        WHERE owner = USER
        AND table_name NOT LIKE 'BIN$%'
        ORDER BY table_name, trigger_name;
    
    RETURN v_cursor;
END;
/

-- 10. FUNKCE PRO ZÍSKÁNÍ INFORMACÍ O ULOŽENÝCH PROCEDURÁCH A FUNKCÍCH
CREATE OR REPLACE FUNCTION f_get_procedures_info
RETURN SYS_REFCURSOR
IS
    v_cursor SYS_REFCURSOR;
BEGIN
    OPEN v_cursor FOR
        SELECT 
            o.object_name AS nazev,
            CASE o.object_type
                WHEN 'PROCEDURE' THEN 'PROCEDURA'
                WHEN 'FUNCTION' THEN 'FUNKCE'
                WHEN 'PACKAGE' THEN 'BALÍČEK'
                WHEN 'PACKAGE BODY' THEN 'TĚLO BALÍČKU'
                ELSE o.object_type
            END AS typ,
            CASE o.status
                WHEN 'VALID' THEN 'PLATNÝ'
                ELSE 'NEPLATNÝ'
            END AS stav,
            o.created AS datum_vytvoreni,
            o.last_ddl_time AS posledni_zmena,
            NVL((SELECT COUNT(DISTINCT a.argument_name) 
                 FROM user_arguments a 
                 WHERE a.object_name = o.object_name 
                 AND a.package_name IS NULL
                 AND a.argument_name IS NOT NULL
                 AND a.data_level = 0), 0) AS pocet_parametru
        FROM user_objects o
        WHERE o.object_type IN ('PROCEDURE', 'FUNCTION', 'PACKAGE', 'PACKAGE BODY')
        ORDER BY o.object_type, o.object_name;
    
    RETURN v_cursor;
END;
/