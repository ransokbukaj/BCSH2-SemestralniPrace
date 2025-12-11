CREATE OR REPLACE FUNCTION f_trzba_z_vystavy(
    p_idvystava IN vystavy.idvystava%TYPE
) RETURN NUMBER
IS
    v_trzba_vstupne NUMBER(12,2);
BEGIN
    -- Tržba ze vstupného na danou výstavu
    SELECT NVL(SUM(dn.cena), 0)
    INTO   v_trzba_vstupne
    FROM   navstevy n
           JOIN druhy_navstev dn
             ON n.iddruhnavstevy = dn.iddruhnavstevy
    WHERE  n.idvystava = p_idvystava;

    RETURN v_trzba_vstupne;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RETURN 0;
END;
/

CREATE OR REPLACE FUNCTION f_prumerna_cena_dil_autora(
    p_idumelec IN umelci.idumelec%TYPE
) RETURN NUMBER
IS
    v_prumer NUMBER(12,2);
BEGIN
    SELECT NVL(AVG(p.cena), 0)
    INTO   v_prumer
    FROM   umelci_umelecka_dila ud
           JOIN umelecka_dila d
             ON ud.idumeleckedilo = d.idumeleckedilo
           JOIN prodeje p
             ON d.idprodej = p.idprodej
    WHERE  ud.idumelec = p_idumelec;

    RETURN v_prumer;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RETURN 0;
END;
/



CREATE OR REPLACE FUNCTION f_popis_vystavy(
    p_idvystava IN vystavy.idvystava%TYPE
) RETURN VARCHAR2
IS
    v_nazev        vystavy.nazev%TYPE;
    v_datumod      vystavy.datumod%TYPE;
    v_datumdo      vystavy.datumdo%TYPE;
    v_pocet_del    NUMBER;
    v_pocet_navst  NUMBER;
BEGIN
    -- Základní údaje o výstavě
    SELECT v.nazev,
           v.datumod,
           v.datumdo,
           (SELECT COUNT(*)
              FROM umelecka_dila d
             WHERE d.idvystava = v.idvystava),
           (SELECT COUNT(*)
              FROM navstevy n
             WHERE n.idvystava = v.idvystava)
    INTO   v_nazev,
           v_datumod,
           v_datumdo,
           v_pocet_del,
           v_pocet_navst
    FROM   vystavy v
    WHERE  v.idvystava = p_idvystava;

    RETURN 'Výstava "' || v_nazev || '" (' ||
           TO_CHAR(v_datumod, 'DD.MM.YYYY') || ' – ' ||
           TO_CHAR(v_datumdo, 'DD.MM.YYYY') || '), počet děl: ' ||
           v_pocet_del || ', počet návštěv: ' || v_pocet_navst;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RETURN 'Výstava s ID=' || p_idvystava || ' neexistuje.';
END;
/


CREATE OR REPLACE FUNCTION f_pocet_prodanych_del_autora(
    p_idumelec IN umelci.idumelec%TYPE
) RETURN NUMBER
IS
    v_pocet NUMBER := 0;

    CURSOR c_dila IS
        SELECT d.idprodej
        FROM umelecka_dila d
             JOIN umelci_umelecka_dila ud ON d.idumeleckedilo = ud.idumeleckedilo
        WHERE ud.idumelec = p_idumelec;
BEGIN
    FOR r IN c_dila LOOP
        IF r.idprodej IS NOT NULL THEN
            v_pocet := v_pocet + 1;
        END IF;
    END LOOP;

    RETURN v_pocet;
END;
/