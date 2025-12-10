CREATE OR REPLACE TRIGGER trg_adresy_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON adresy
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
    v_posta VARCHAR2(100);
    v_posta_old VARCHAR2(100);
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.idadresa;
        
        -- Načtení města a PSČ
        SELECT obec || ' ' || psc INTO v_posta
        FROM posty
        WHERE idposta = :NEW.idposta;
        
        v_popiszmeny := 'Přidána nová adresa: ' || :NEW.ulice || ' ' || :NEW.cislopopisne;
        
        v_novehodnoty := 
            'ulice: ' || :NEW.ulice || ', ' ||
            'cislopopisne: ' || :NEW.cislopopisne || ', ' ||
            'cisloorientacni: ' || NVL(:NEW.cisloorientacni, 'NULL') || ', ' ||
            'posta: ' || v_posta;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idadresa;
        
        -- Načtení měst a PSČ
        SELECT obec || ' ' || psc INTO v_posta_old
        FROM posty
        WHERE idposta = :OLD.idposta;
        
        SELECT obec || ' ' || psc INTO v_posta
        FROM posty
        WHERE idposta = :NEW.idposta;
        
        v_popiszmeny := 'Aktualizace adresy: ' || :NEW.ulice || ' ' || :NEW.cislopopisne;
        
        v_starehodnoty := 
            'ulice: ' || :OLD.ulice || ', ' ||
            'cislopopisne: ' || :OLD.cislopopisne || ', ' ||
            'cisloorientacni: ' || NVL(:OLD.cisloorientacni, 'NULL') || ', ' ||
            'posta: ' || v_posta_old;
            
        v_novehodnoty := 
            'ulice: ' || :NEW.ulice || ', ' ||
            'cislopopisne: ' || :NEW.cislopopisne || ', ' ||
            'cisloorientacni: ' || NVL(:NEW.cisloorientacni, 'NULL') || ', ' ||
            'posta: ' || v_posta;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idadresa;
        
        -- Načtení města a PSČ
        SELECT obec || ' ' || psc INTO v_posta
        FROM posty
        WHERE idposta = :OLD.idposta;
        
        v_popiszmeny := 'Smazána adresa: ' || :OLD.ulice || ' ' || :OLD.cislopopisne;
        
        v_starehodnoty := 
            'ulice: ' || :OLD.ulice || ', ' ||
            'cislopopisne: ' || :OLD.cislopopisne || ', ' ||
            'cisloorientacni: ' || NVL(:OLD.cisloorientacni, 'NULL') || ', ' ||
            'posta: ' || v_posta;
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'ADRESY',
        v_idradku,
        v_iduzivatel
    );
END;
/

CREATE OR REPLACE TRIGGER trg_uzivatele_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON uzivatele
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
    v_nazev_role role.nazev%TYPE;
    v_nazev_role_old role.nazev%TYPE;
    v_log_change BOOLEAN := TRUE;
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (registrace/systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    IF UPDATING THEN
        IF (:NEW.uzivatelskejmeno = :OLD.uzivatelskejmeno AND
            :NEW.heslohash = :OLD.heslohash AND
            :NEW.jmeno = :OLD.jmeno AND
            :NEW.prijmeni = :OLD.prijmeni AND
            NVL(:NEW.email, 'NULL') = NVL(:OLD.email, 'NULL') AND
            NVL(:NEW.telefonicislo, 'NULL') = NVL(:OLD.telefonicislo, 'NULL') AND
            :NEW.datumregistrace = :OLD.datumregistrace AND
            :NEW.deaktivovan = :OLD.deaktivovan AND
            :NEW.idrole = :OLD.idrole AND
            (:NEW.datumposlednihoprihlaseni != :OLD.datumposlednihoprihlaseni OR 
            (:NEW.datumposlednihoprihlaseni IS NOT NULL AND :OLD.datumposlednihoprihlaseni IS NULL))) THEN
            v_log_change := FALSE;
        END IF;
    END IF;
    
    -- Pokud nemáme logovat, skonči
    IF NOT v_log_change THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.iduzivatel;
        
        -- Načtení názvu role
        SELECT nazev INTO v_nazev_role
        FROM role
        WHERE idrole = :NEW.idrole;
        
        v_popiszmeny := 'Přidán nový uživatel: ' || :NEW.uzivatelskejmeno;
        
        v_novehodnoty := 
            'uzivatelskejmeno: ' || :NEW.uzivatelskejmeno || ', ' ||
            'jmeno: ' || :NEW.jmeno || ', ' ||
            'prijmeni: ' || :NEW.prijmeni || ', ' ||
            'email: ' || NVL(:NEW.email, 'NULL') || ', ' ||
            'telefonicislo: ' || NVL(:NEW.telefonicislo, 'NULL') || ', ' ||
            'datumregistrace: ' || TO_CHAR(:NEW.datumregistrace, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'deaktivovan: ' || :NEW.deaktivovan || ', ' ||
            'role: ' || v_nazev_role;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.iduzivatel;
        
        -- Načtení názvů rolí
        SELECT nazev INTO v_nazev_role_old
        FROM role
        WHERE idrole = :OLD.idrole;
        
        SELECT nazev INTO v_nazev_role
        FROM role
        WHERE idrole = :NEW.idrole;
        
        v_popiszmeny := 'Aktualizace uživatele: ' || :NEW.uzivatelskejmeno;
        
        v_starehodnoty := 
            'uzivatelskejmeno: ' || :OLD.uzivatelskejmeno || ', ' ||
            'jmeno: ' || :OLD.jmeno || ', ' ||
            'prijmeni: ' || :OLD.prijmeni || ', ' ||
            'email: ' || NVL(:OLD.email, 'NULL') || ', ' ||
            'telefonicislo: ' || NVL(:OLD.telefonicislo, 'NULL') || ', ' ||
            'datumregistrace: ' || TO_CHAR(:OLD.datumregistrace, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'datumposlednihoprihlaseni: ' || NVL(TO_CHAR(:OLD.datumposlednihoprihlaseni, 'DD.MM.YYYY HH24:MI:SS'), 'NULL') || ', ' ||
            'deaktivovan: ' || :OLD.deaktivovan || ', ' ||
            'role: ' || v_nazev_role_old;
            
        v_novehodnoty := 
            'uzivatelskejmeno: ' || :NEW.uzivatelskejmeno || ', ' ||
            'jmeno: ' || :NEW.jmeno || ', ' ||
            'prijmeni: ' || :NEW.prijmeni || ', ' ||
            'email: ' || NVL(:NEW.email, 'NULL') || ', ' ||
            'telefonicislo: ' || NVL(:NEW.telefonicislo, 'NULL') || ', ' ||
            'datumregistrace: ' || TO_CHAR(:NEW.datumregistrace, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'datumposlednihoprihlaseni: ' || NVL(TO_CHAR(:NEW.datumposlednihoprihlaseni, 'DD.MM.YYYY HH24:MI:SS'), 'NULL') || ', ' ||
            'deaktivovan: ' || :NEW.deaktivovan || ', ' ||
            'role: ' || v_nazev_role;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.iduzivatel;
        
        -- Načtení názvu role
        SELECT nazev INTO v_nazev_role
        FROM role
        WHERE idrole = :OLD.idrole;
        
        v_popiszmeny := 'Smazán/deaktivován uživatel: ' || :OLD.uzivatelskejmeno;
        
        v_starehodnoty := 
            'uzivatelskejmeno: ' || :OLD.uzivatelskejmeno || ', ' ||
            'jmeno: ' || :OLD.jmeno || ', ' ||
            'prijmeni: ' || :OLD.prijmeni || ', ' ||
            'email: ' || NVL(:OLD.email, 'NULL') || ', ' ||
            'telefonicislo: ' || NVL(:OLD.telefonicislo, 'NULL') || ', ' ||
            'datumregistrace: ' || TO_CHAR(:OLD.datumregistrace, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'datumposlednihoprihlaseni: ' || NVL(TO_CHAR(:OLD.datumposlednihoprihlaseni, 'DD.MM.YYYY HH24:MI:SS'), 'NULL') || ', ' ||
            'deaktivovan: ' || :OLD.deaktivovan || ', ' ||
            'role: ' || v_nazev_role;
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'UZIVATELE',
        v_idradku,
        v_iduzivatel
    );
END;
/

CREATE OR REPLACE TRIGGER trg_posty_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON posty
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.idposta;
        v_popiszmeny := 'Přidána nová pošta: ' || :NEW.obec || ' (' || :NEW.psc || ')';
        
        v_novehodnoty := 
            'obec: ' || :NEW.obec || ', ' ||
            'psc: ' || :NEW.psc;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idposta;
        v_popiszmeny := 'Aktualizace pošty: ' || :NEW.obec || ' (' || :NEW.psc || ')';
        
        v_starehodnoty := 
            'obec: ' || :OLD.obec || ', ' ||
            'psc: ' || :OLD.psc;
            
        v_novehodnoty := 
            'obec: ' || :NEW.obec || ', ' ||
            'psc: ' || :NEW.psc;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idposta;
        v_popiszmeny := 'Smazána pošta: ' || :OLD.obec || ' (' || :OLD.psc || ')';
        
        v_starehodnoty := 
            'obec: ' || :OLD.obec || ', ' ||
            'psc: ' || :OLD.psc;
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'POSTY',
        v_idradku,
        v_iduzivatel
    );
END;
/

CREATE OR REPLACE TRIGGER trg_kupci_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON kupci
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.idkupec;
        v_popiszmeny := 'Přidán nový kupec: ' || :NEW.prijmeni || ' ' || :NEW.jmeno;
        
        v_novehodnoty := 
            'jmeno: ' || :NEW.jmeno || ', ' ||
            'prijmeni: ' || :NEW.prijmeni || ', ' ||
            'telefonicislo: ' || :NEW.telefonicislo || ', ' ||
            'email: ' || NVL(:NEW.email, 'NULL') || ', ' ||
            'idadresa: ' || :NEW.idadresa;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idkupec;
        v_popiszmeny := 'Aktualizace kupce: ' || :NEW.prijmeni || ' ' || :NEW.jmeno;
        
        v_starehodnoty := 
            'jmeno: ' || :OLD.jmeno || ', ' ||
            'prijmeni: ' || :OLD.prijmeni || ', ' ||
            'telefonicislo: ' || :OLD.telefonicislo || ', ' ||
            'email: ' || NVL(:OLD.email, 'NULL') || ', ' ||
            'idadresa: ' || :OLD.idadresa;
            
        v_novehodnoty := 
            'jmeno: ' || :NEW.jmeno || ', ' ||
            'prijmeni: ' || :NEW.prijmeni || ', ' ||
            'telefonicislo: ' || :NEW.telefonicislo || ', ' ||
            'email: ' || NVL(:NEW.email, 'NULL') || ', ' ||
            'idadresa: ' || :NEW.idadresa;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idkupec;
        v_popiszmeny := 'Smazán kupec: ' || :OLD.prijmeni || ' ' || :OLD.jmeno;
        
        v_starehodnoty := 
            'jmeno: ' || :OLD.jmeno || ', ' ||
            'prijmeni: ' || :OLD.prijmeni || ', ' ||
            'telefonicislo: ' || :OLD.telefonicislo || ', ' ||
            'email: ' || NVL(:OLD.email, 'NULL') || ', ' ||
            'idadresa: ' || :OLD.idadresa;
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'KUPCI',
        v_idradku,
        v_iduzivatel
    );
END;
/

CREATE OR REPLACE TRIGGER trg_prodeje_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON prodeje
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
    v_nazev_platby druhy_plateb.nazev%TYPE;
    v_nazev_platby_old druhy_plateb.nazev%TYPE;
    v_jmeno_kupce VARCHAR2(200);
    v_jmeno_kupce_old VARCHAR2(200);
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.idprodej;
        
        -- Načtení názvu druhu platby
        SELECT nazev INTO v_nazev_platby
        FROM druhy_plateb
        WHERE iddruhplatby = :NEW.iddruhplatby;
        
        -- Načtení jména a příjmení kupce
        SELECT prijmeni || ' ' || jmeno INTO v_jmeno_kupce
        FROM kupci
        WHERE idkupec = :NEW.idkupec;
        
        v_popiszmeny := 'Přidán nový prodej: ' || TO_CHAR(:NEW.cena, '999999999.99') || ' Kč, datum ' || TO_CHAR(:NEW.datumprodeje, 'DD.MM.YYYY HH24:MI') || ', kupec ' || v_jmeno_kupce || ', platba ' || v_nazev_platby;
        
        v_novehodnoty := 
            'cena: ' || TO_CHAR(:NEW.cena, '999999999.99') || ', ' ||
            'datumprodeje: ' || TO_CHAR(:NEW.datumprodeje, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'cislokarty: ' || NVL(:NEW.cislokarty, 'NULL') || ', ' ||
            'cislouctu: ' || NVL(:NEW.cislouctu, 'NULL') || ', ' ||
            'druh_platby: ' || v_nazev_platby || ', ' ||
            'kupec: ' || v_jmeno_kupce;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idprodej;
        
        -- Načtení názvů druhů platby
        SELECT nazev INTO v_nazev_platby_old
        FROM druhy_plateb
        WHERE iddruhplatby = :OLD.iddruhplatby;
        
        SELECT nazev INTO v_nazev_platby
        FROM druhy_plateb
        WHERE iddruhplatby = :NEW.iddruhplatby;
        
        -- Načtení jmen kupců
        SELECT prijmeni || ' ' || jmeno INTO v_jmeno_kupce_old
        FROM kupci
        WHERE idkupec = :OLD.idkupec;
        
        SELECT prijmeni || ' ' || jmeno INTO v_jmeno_kupce
        FROM kupci
        WHERE idkupec = :NEW.idkupec;
        
        v_popiszmeny := 'Aktualizace prodeje ID: ' || :NEW.idprodej;
        
        v_starehodnoty := 
            'cena: ' || TO_CHAR(:OLD.cena, '999999999.99') || ', ' ||
            'datumprodeje: ' || TO_CHAR(:OLD.datumprodeje, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'cislokarty: ' || NVL(:OLD.cislokarty, 'NULL') || ', ' ||
            'cislouctu: ' || NVL(:OLD.cislouctu, 'NULL') || ', ' ||
            'druh_platby: ' || v_nazev_platby_old || ', ' ||
            'kupec: ' || v_jmeno_kupce_old;
            
        v_novehodnoty := 
            'cena: ' || TO_CHAR(:NEW.cena, '999999999.99') || ', ' ||
            'datumprodeje: ' || TO_CHAR(:NEW.datumprodeje, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'cislokarty: ' || NVL(:NEW.cislokarty, 'NULL') || ', ' ||
            'cislouctu: ' || NVL(:NEW.cislouctu, 'NULL') || ', ' ||
            'druh_platby: ' || v_nazev_platby || ', ' ||
            'kupec: ' || v_jmeno_kupce;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idprodej;
        
        -- Načtení názvu druhu platby
        SELECT nazev INTO v_nazev_platby
        FROM druhy_plateb
        WHERE iddruhplatby = :OLD.iddruhplatby;
        
        -- Načtení jména a příjmení kupce
        SELECT prijmeni || ' ' || jmeno INTO v_jmeno_kupce
        FROM kupci
        WHERE idkupec = :OLD.idkupec;
        
        v_popiszmeny := 'Smazán prodej: ' || TO_CHAR(:OLD.cena, '999999999.99') || ' Kč, datum ' || TO_CHAR(:OLD.datumprodeje, 'DD.MM.YYYY HH24:MI') || ', kupec ' || v_jmeno_kupce || ', platba ' || v_nazev_platby;
        
        v_starehodnoty := 
            'cena: ' || TO_CHAR(:OLD.cena, '999999999.99') || ', ' ||
            'datumprodeje: ' || TO_CHAR(:OLD.datumprodeje, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'cislokarty: ' || NVL(:OLD.cislokarty, 'NULL') || ', ' ||
            'cislouctu: ' || NVL(:OLD.cislouctu, 'NULL') || ', ' ||
            'druh_platby: ' || v_nazev_platby || ', ' ||
            'kupec: ' || v_jmeno_kupce;
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'PRODEJE',
        v_idradku,
        v_iduzivatel
    );
END;
/

CREATE OR REPLACE TRIGGER trg_vystavy_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON vystavy
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
    v_nazev_programu vzdelavaci_programy.nazev%TYPE;
    v_nazev_programu_old vzdelavaci_programy.nazev%TYPE;
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.idvystava;
        v_popiszmeny := 'Přidána nová výstava: ' || :NEW.nazev;
        
        -- Načtení názvu vzdělávacího programu (pokud existuje)
        IF :NEW.idvzdelavaciprogram IS NOT NULL THEN
            SELECT nazev INTO v_nazev_programu
            FROM vzdelavaci_programy
            WHERE idvzdelavaciprogram = :NEW.idvzdelavaciprogram;
        ELSE
            v_nazev_programu := NULL;
        END IF;
        
        v_novehodnoty := 
            'nazev: ' || :NEW.nazev || ', ' ||
            'datumod: ' || TO_CHAR(:NEW.datumod, 'DD.MM.YYYY') || ', ' ||
            'datumdo: ' || TO_CHAR(:NEW.datumdo, 'DD.MM.YYYY') || ', ' ||
            'popis: ' || SUBSTR(:NEW.popis, 1, 200) || ', ' ||
            'vzdelavaci_program: ' || NVL(v_nazev_programu, 'NULL');
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idvystava;
        v_popiszmeny := 'Aktualizace výstavy: ' || :NEW.nazev;
        
        -- Načtení názvů vzdělávacích programů
        IF :OLD.idvzdelavaciprogram IS NOT NULL THEN
            SELECT nazev INTO v_nazev_programu_old
            FROM vzdelavaci_programy
            WHERE idvzdelavaciprogram = :OLD.idvzdelavaciprogram;
        ELSE
            v_nazev_programu_old := NULL;
        END IF;
        
        IF :NEW.idvzdelavaciprogram IS NOT NULL THEN
            SELECT nazev INTO v_nazev_programu
            FROM vzdelavaci_programy
            WHERE idvzdelavaciprogram = :NEW.idvzdelavaciprogram;
        ELSE
            v_nazev_programu := NULL;
        END IF;
        
        v_starehodnoty := 
            'nazev: ' || :OLD.nazev || ', ' ||
            'datumod: ' || TO_CHAR(:OLD.datumod, 'DD.MM.YYYY') || ', ' ||
            'datumdo: ' || TO_CHAR(:OLD.datumdo, 'DD.MM.YYYY') || ', ' ||
            'popis: ' || SUBSTR(:OLD.popis, 1, 200) || ', ' ||
            'vzdelavaci_program: ' || NVL(v_nazev_programu_old, 'NULL');
            
        v_novehodnoty := 
            'nazev: ' || :NEW.nazev || ', ' ||
            'datumod: ' || TO_CHAR(:NEW.datumod, 'DD.MM.YYYY') || ', ' ||
            'datumdo: ' || TO_CHAR(:NEW.datumdo, 'DD.MM.YYYY') || ', ' ||
            'popis: ' || SUBSTR(:NEW.popis, 1, 200) || ', ' ||
            'vzdelavaci_program: ' || NVL(v_nazev_programu, 'NULL');
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idvystava;
        v_popiszmeny := 'Smazána výstava: ' || :OLD.nazev;
        
        -- Načtení názvu vzdělávacího programu (pokud existuje)
        IF :OLD.idvzdelavaciprogram IS NOT NULL THEN
            SELECT nazev INTO v_nazev_programu
            FROM vzdelavaci_programy
            WHERE idvzdelavaciprogram = :OLD.idvzdelavaciprogram;
        ELSE
            v_nazev_programu := NULL;
        END IF;
        
        v_starehodnoty := 
            'nazev: ' || :OLD.nazev || ', ' ||
            'datumod: ' || TO_CHAR(:OLD.datumod, 'DD.MM.YYYY') || ', ' ||
            'datumdo: ' || TO_CHAR(:OLD.datumdo, 'DD.MM.YYYY') || ', ' ||
            'popis: ' || SUBSTR(:OLD.popis, 1, 200) || ', ' ||
            'vzdelavaci_program: ' || NVL(v_nazev_programu, 'NULL');
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'VYSTAVY',
        v_idradku,
        v_iduzivatel
    );
END;
/

CREATE OR REPLACE TRIGGER trg_vzdelavaci_programy_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON vzdelavaci_programy
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.idvzdelavaciprogram;
        v_popiszmeny := 'Přidán nový vzdělávací program: ' || :NEW.nazev;
        
        v_novehodnoty := 
            'nazev: ' || :NEW.nazev || ', ' ||
            'datumod: ' || TO_CHAR(:NEW.datumod, 'DD.MM.YYYY') || ', ' ||
            'datumdo: ' || TO_CHAR(:NEW.datumdo, 'DD.MM.YYYY') || ', ' ||
            'popis: ' || SUBSTR(:NEW.popis, 1, 200);
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idvzdelavaciprogram;
        v_popiszmeny := 'Aktualizace vzdělávacího programu: ' || :NEW.nazev;
        
        v_starehodnoty := 
            'nazev: ' || :OLD.nazev || ', ' ||
            'datumod: ' || TO_CHAR(:OLD.datumod, 'DD.MM.YYYY') || ', ' ||
            'datumdo: ' || TO_CHAR(:OLD.datumdo, 'DD.MM.YYYY') || ', ' ||
            'popis: ' || SUBSTR(:OLD.popis, 1, 200);
            
        v_novehodnoty := 
            'nazev: ' || :NEW.nazev || ', ' ||
            'datumod: ' || TO_CHAR(:NEW.datumod, 'DD.MM.YYYY') || ', ' ||
            'datumdo: ' || TO_CHAR(:NEW.datumdo, 'DD.MM.YYYY') || ', ' ||
            'popis: ' || SUBSTR(:NEW.popis, 1, 200);
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idvzdelavaciprogram;
        v_popiszmeny := 'Smazán vzdělávací program: ' || :OLD.nazev;
        
        v_starehodnoty := 
            'nazev: ' || :OLD.nazev || ', ' ||
            'datumod: ' || TO_CHAR(:OLD.datumod, 'DD.MM.YYYY') || ', ' ||
            'datumdo: ' || TO_CHAR(:OLD.datumdo, 'DD.MM.YYYY') || ', ' ||
            'popis: ' || SUBSTR(:OLD.popis, 1, 200);
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'VZDELAVACI_PROGRAMY',
        v_idradku,
        v_iduzivatel
    );
END;
/

CREATE OR REPLACE TRIGGER trg_navstevy_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON navstevy
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
    v_nazev_vystavy vystavy.nazev%TYPE;
    v_nazev_vystavy_old vystavy.nazev%TYPE;
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.idnavsteva;
        
        -- Načtení názvu výstavy
        SELECT nazev INTO v_nazev_vystavy
        FROM vystavy
        WHERE idvystava = :NEW.idvystava;
        
        v_popiszmeny := 'Přidána nová návštěva: ' || TO_CHAR(:NEW.datumnavstevy, 'DD.MM.YYYY HH24:MI') || ' na výstavě ' || v_nazev_vystavy;
        
        v_novehodnoty := 
            'datumnavstevy: ' || TO_CHAR(:NEW.datumnavstevy, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'iddruhnavstevy: ' || :NEW.iddruhnavstevy || ', ' ||
            'vystava: ' || v_nazev_vystavy;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idnavsteva;
        
        -- Načtení názvů výstav
        SELECT nazev INTO v_nazev_vystavy_old
        FROM vystavy
        WHERE idvystava = :OLD.idvystava;
        
        SELECT nazev INTO v_nazev_vystavy
        FROM vystavy
        WHERE idvystava = :NEW.idvystava;
        
        v_popiszmeny := 'Aktualizace návštěvy ID: ' || :NEW.idnavsteva;
        
        v_starehodnoty := 
            'datumnavstevy: ' || TO_CHAR(:OLD.datumnavstevy, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'iddruhnavstevy: ' || :OLD.iddruhnavstevy || ', ' ||
            'vystava: ' || v_nazev_vystavy_old;
            
        v_novehodnoty := 
            'datumnavstevy: ' || TO_CHAR(:NEW.datumnavstevy, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'iddruhnavstevy: ' || :NEW.iddruhnavstevy || ', ' ||
            'vystava: ' || v_nazev_vystavy;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idnavsteva;
        
        -- Načtení názvu výstavy
        SELECT nazev INTO v_nazev_vystavy
        FROM vystavy
        WHERE idvystava = :OLD.idvystava;
        
        v_popiszmeny := 'Smazána návštěva: ' || TO_CHAR(:OLD.datumnavstevy, 'DD.MM.YYYY HH24:MI') || ' na výstavě ' || v_nazev_vystavy;
        
        v_starehodnoty := 
            'datumnavstevy: ' || TO_CHAR(:OLD.datumnavstevy, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'iddruhnavstevy: ' || :OLD.iddruhnavstevy || ', ' ||
            'vystava: ' || v_nazev_vystavy;
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'NAVSTEVY',
        v_idradku,
        v_iduzivatel
    );
END;
/

CREATE OR REPLACE TRIGGER trg_umelci_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON umelci
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.idumelec;
        v_popiszmeny := 'Přidán nový umělec: ' || :NEW.prijmeni || ' ' || :NEW.jmeno;
        
        v_novehodnoty := 
            'jmeno: ' || :NEW.jmeno || ', ' ||
            'prijmeni: ' || :NEW.prijmeni || ', ' ||
            'datumnarozeni: ' || TO_CHAR(:NEW.datumnarozeni, 'DD.MM.YYYY') || ', ' ||
            'datumumrti: ' || NVL(TO_CHAR(:NEW.datumumrti, 'DD.MM.YYYY'), 'NULL') || ', ' ||
            'popis: ' || SUBSTR(:NEW.popis, 1, 200);
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idumelec;
        v_popiszmeny := 'Aktualizace umělce: ' || :NEW.prijmeni || ' ' || :NEW.jmeno;
        
        v_starehodnoty := 
            'jmeno: ' || :OLD.jmeno || ', ' ||
            'prijmeni: ' || :OLD.prijmeni || ', ' ||
            'datumnarozeni: ' || TO_CHAR(:OLD.datumnarozeni, 'DD.MM.YYYY') || ', ' ||
            'datumumrti: ' || NVL(TO_CHAR(:OLD.datumumrti, 'DD.MM.YYYY'), 'NULL') || ', ' ||
            'popis: ' || SUBSTR(:OLD.popis, 1, 200);
            
        v_novehodnoty := 
            'jmeno: ' || :NEW.jmeno || ', ' ||
            'prijmeni: ' || :NEW.prijmeni || ', ' ||
            'datumnarozeni: ' || TO_CHAR(:NEW.datumnarozeni, 'DD.MM.YYYY') || ', ' ||
            'datumumrti: ' || NVL(TO_CHAR(:NEW.datumumrti, 'DD.MM.YYYY'), 'NULL') || ', ' ||
            'popis: ' || SUBSTR(:NEW.popis, 1, 200);
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idumelec;
        v_popiszmeny := 'Smazán umělec: ' || :OLD.prijmeni || ' ' || :OLD.jmeno;
        
        v_starehodnoty := 
            'jmeno: ' || :OLD.jmeno || ', ' ||
            'prijmeni: ' || :OLD.prijmeni || ', ' ||
            'datumnarozeni: ' || TO_CHAR(:OLD.datumnarozeni, 'DD.MM.YYYY') || ', ' ||
            'datumumrti: ' || NVL(TO_CHAR(:OLD.datumumrti, 'DD.MM.YYYY'), 'NULL') || ', ' ||
            'popis: ' || SUBSTR(:OLD.popis, 1, 200);
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'UMELCI',
        v_idradku,
        v_iduzivatel
    );
END;
/

-- Trigger pro tabulku umelecka_dila
CREATE OR REPLACE TRIGGER trg_umelecka_dila_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON umelecka_dila
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.idumeleckedilo;
        v_popiszmeny := 'Přidáno nové umělecké dílo: ' || :NEW.nazev;
        
        v_novehodnoty := 
            'nazev: ' || :NEW.nazev || ', ' ||
            'popis: ' || SUBSTR(:NEW.popis, 1, 200) || ', ' ||
            'datumzverejneni: ' || TO_CHAR(:NEW.datumzverejneni, 'DD.MM.YYYY') || ', ' ||
            'vyska: ' || TO_CHAR(:NEW.vyska, '999999.99') || ', ' ||
            'sirka: ' || TO_CHAR(:NEW.sirka, '999999.99') || ', ' ||
            'idprodej: ' || NVL(TO_CHAR(:NEW.idprodej), 'NULL') || ', ' ||
            'idvystava: ' || NVL(TO_CHAR(:NEW.idvystava), 'NULL') || ', ' ||
            'typdila: ' || :NEW.typdila;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idumeleckedilo;
        v_popiszmeny := 'Aktualizace uměleckého díla: ' || :NEW.nazev;
        
        v_starehodnoty := 
            'nazev: ' || :OLD.nazev || ', ' ||
            'popis: ' || SUBSTR(:OLD.popis, 1, 200) || ', ' ||
            'datumzverejneni: ' || TO_CHAR(:OLD.datumzverejneni, 'DD.MM.YYYY') || ', ' ||
            'vyska: ' || TO_CHAR(:OLD.vyska, '999999.99') || ', ' ||
            'sirka: ' || TO_CHAR(:OLD.sirka, '999999.99') || ', ' ||
            'idprodej: ' || NVL(TO_CHAR(:OLD.idprodej), 'NULL') || ', ' ||
            'idvystava: ' || NVL(TO_CHAR(:OLD.idvystava), 'NULL') || ', ' ||
            'typdila: ' || :OLD.typdila;
            
        v_novehodnoty := 
            'nazev: ' || :NEW.nazev || ', ' ||
            'popis: ' || SUBSTR(:NEW.popis, 1, 200) || ', ' ||
            'datumzverejneni: ' || TO_CHAR(:NEW.datumzverejneni, 'DD.MM.YYYY') || ', ' ||
            'vyska: ' || TO_CHAR(:NEW.vyska, '999999.99') || ', ' ||
            'sirka: ' || TO_CHAR(:NEW.sirka, '999999.99') || ', ' ||
            'idprodej: ' || NVL(TO_CHAR(:NEW.idprodej), 'NULL') || ', ' ||
            'idvystava: ' || NVL(TO_CHAR(:NEW.idvystava), 'NULL') || ', ' ||
            'typdila: ' || :NEW.typdila;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idumeleckedilo;
        v_popiszmeny := 'Smazáno umělecké dílo: ' || :OLD.nazev;
        
        v_starehodnoty := 
            'nazev: ' || :OLD.nazev || ', ' ||
            'popis: ' || SUBSTR(:OLD.popis, 1, 200) || ', ' ||
            'datumzverejneni: ' || TO_CHAR(:OLD.datumzverejneni, 'DD.MM.YYYY') || ', ' ||
            'vyska: ' || TO_CHAR(:OLD.vyska, '999999.99') || ', ' ||
            'sirka: ' || TO_CHAR(:OLD.sirka, '999999.99') || ', ' ||
            'idprodej: ' || NVL(TO_CHAR(:OLD.idprodej), 'NULL') || ', ' ||
            'idvystava: ' || NVL(TO_CHAR(:OLD.idvystava), 'NULL') || ', ' ||
            'typdila: ' || :OLD.typdila;
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'UMELECKA_DILA',
        v_idradku,
        v_iduzivatel
    );
END;
/

-- Trigger pro tabulku obrazy
CREATE OR REPLACE TRIGGER trg_obrazy_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON obrazy
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.idumeleckedilo;
        v_popiszmeny := 'Přidány specifické údaje obrazu ID: ' || :NEW.idumeleckedilo;
        
        v_novehodnoty := 
            'idumeleckedilo: ' || :NEW.idumeleckedilo || ', ' ||
            'idpodklad: ' || :NEW.idpodklad || ', ' ||
            'idtechnika: ' || :NEW.idtechnika;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idumeleckedilo;
        v_popiszmeny := 'Aktualizace specifických údajů obrazu ID: ' || :NEW.idumeleckedilo;
        
        v_starehodnoty := 
            'idumeleckedilo: ' || :OLD.idumeleckedilo || ', ' ||
            'idpodklad: ' || :OLD.idpodklad || ', ' ||
            'idtechnika: ' || :OLD.idtechnika;
            
        v_novehodnoty := 
            'idumeleckedilo: ' || :NEW.idumeleckedilo || ', ' ||
            'idpodklad: ' || :NEW.idpodklad || ', ' ||
            'idtechnika: ' || :NEW.idtechnika;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idumeleckedilo;
        v_popiszmeny := 'Smazány specifické údaje obrazu ID: ' || :OLD.idumeleckedilo;
        
        v_starehodnoty := 
            'idumeleckedilo: ' || :OLD.idumeleckedilo || ', ' ||
            'idpodklad: ' || :OLD.idpodklad || ', ' ||
            'idtechnika: ' || :OLD.idtechnika;
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'OBRAZY',
        v_idradku,
        v_iduzivatel
    );
END;
/

-- Trigger pro tabulku sochy
CREATE OR REPLACE TRIGGER trg_sochy_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON sochy
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.idumeleckedilo;
        v_popiszmeny := 'Přidány specifické údaje sochy ID: ' || :NEW.idumeleckedilo;
        
        v_novehodnoty := 
            'idumeleckedilo: ' || :NEW.idumeleckedilo || ', ' ||
            'hloubka: ' || :NEW.hloubka || ', ' ||
            'hmotnost: ' || TO_CHAR(:NEW.hmotnost, '999999.99') || ', ' ||
            'idmaterial: ' || :NEW.idmaterial;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idumeleckedilo;
        v_popiszmeny := 'Aktualizace specifických údajů sochy ID: ' || :NEW.idumeleckedilo;
        
        v_starehodnoty := 
            'idumeleckedilo: ' || :OLD.idumeleckedilo || ', ' ||
            'hloubka: ' || :OLD.hloubka || ', ' ||
            'hmotnost: ' || TO_CHAR(:OLD.hmotnost, '999999.99') || ', ' ||
            'idmaterial: ' || :OLD.idmaterial;
            
        v_novehodnoty := 
            'idumeleckedilo: ' || :NEW.idumeleckedilo || ', ' ||
            'hloubka: ' || :NEW.hloubka || ', ' ||
            'hmotnost: ' || TO_CHAR(:NEW.hmotnost, '999999.99') || ', ' ||
            'idmaterial: ' || :NEW.idmaterial;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idumeleckedilo;
        v_popiszmeny := 'Smazány specifické údaje sochy ID: ' || :OLD.idumeleckedilo;
        
        v_starehodnoty := 
            'idumeleckedilo: ' || :OLD.idumeleckedilo || ', ' ||
            'hloubka: ' || :OLD.hloubka || ', ' ||
            'hmotnost: ' || TO_CHAR(:OLD.hmotnost, '999999.99') || ', ' ||
            'idmaterial: ' || :OLD.idmaterial;
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'SOCHY',
        v_idradku,
        v_iduzivatel
    );
END;
/

CREATE OR REPLACE TRIGGER trg_umelci_dila_historie
    AFTER INSERT OR UPDATE OR DELETE
    ON umelci_umelecka_dila
    FOR EACH ROW
DECLARE
    v_iduzivatel uzivatele.iduzivatel%TYPE;
    v_druhoperace VARCHAR2(10);
    v_starehodnoty CLOB;
    v_novehodnoty CLOB;
    v_popiszmeny VARCHAR2(500);
    v_idradku INTEGER;
    v_jmeno_umelce VARCHAR2(200);
    v_jmeno_umelce_old VARCHAR2(200);
    v_nazev_dila VARCHAR2(250);
    v_nazev_dila_old VARCHAR2(250);
BEGIN
    -- Zjištění ID aktuálního uživatele z SESSION_IDENTIFIER
    BEGIN
        SELECT TO_NUMBER(SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER'))
        INTO v_iduzivatel
        FROM DUAL;
    EXCEPTION
        WHEN OTHERS THEN
            v_iduzivatel := NULL;
    END;
    
    -- Pokud není nastavený identifikátor, nepokračuj (systémové operace)
    IF v_iduzivatel IS NULL THEN
        RETURN;
    END IF;
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.idumeleckedilo;
        
        -- Načtení jména a příjmení umělce
        SELECT prijmeni || ' ' || jmeno INTO v_jmeno_umelce
        FROM umelci
        WHERE idumelec = :NEW.idumelec;
        
        -- Načtení názvu uměleckého díla
        SELECT nazev INTO v_nazev_dila
        FROM umelecka_dila
        WHERE idumeleckedilo = :NEW.idumeleckedilo;
        
        v_popiszmeny := 'Přiřazen umělec k dílu: ' || v_jmeno_umelce || ' - ' || v_nazev_dila;
        
        v_novehodnoty := 
            'umelec: ' || v_jmeno_umelce || ', ' ||
            'umelecke_dilo: ' || v_nazev_dila;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idumeleckedilo;
        
        -- Načtení starých jmen
        SELECT prijmeni || ' ' || jmeno INTO v_jmeno_umelce_old
        FROM umelci
        WHERE idumelec = :OLD.idumelec;
        
        SELECT nazev INTO v_nazev_dila_old
        FROM umelecka_dila
        WHERE idumeleckedilo = :OLD.idumeleckedilo;
        
        -- Načtení nových jmen
        SELECT prijmeni || ' ' || jmeno INTO v_jmeno_umelce
        FROM umelci
        WHERE idumelec = :NEW.idumelec;
        
        SELECT nazev INTO v_nazev_dila
        FROM umelecka_dila
        WHERE idumeleckedilo = :NEW.idumeleckedilo;
        
        v_popiszmeny := 'Aktualizace přiřazení umělce k dílu';
        
        v_starehodnoty := 
            'umelec: ' || v_jmeno_umelce_old || ', ' ||
            'umelecke_dilo: ' || v_nazev_dila_old;
            
        v_novehodnoty := 
            'umelec: ' || v_jmeno_umelce || ', ' ||
            'umelecke_dilo: ' || v_nazev_dila;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idumeleckedilo;
        
        -- Načtení jména a příjmení umělce
        SELECT prijmeni || ' ' || jmeno INTO v_jmeno_umelce
        FROM umelci
        WHERE idumelec = :OLD.idumelec;
        
        -- Načtení názvu uměleckého díla
        SELECT nazev INTO v_nazev_dila
        FROM umelecka_dila
        WHERE idumeleckedilo = :OLD.idumeleckedilo;
        
        v_popiszmeny := 'Odebrán umělec od díla: ' || v_jmeno_umelce || ' - ' || v_nazev_dila;
        
        v_starehodnoty := 
            'umelec: ' || v_jmeno_umelce || ', ' ||
            'umelecke_dilo: ' || v_nazev_dila;
        v_novehodnoty := NULL;
    END IF;
    
    -- Vložení záznamu do historie
    INSERT INTO zaznamy_historie (
        datumzmeny,
        popiszmeny,
        druhoperace,
        starehodnoty,
        novehodnoty,
        nazevtabulky,
        idradkutabulky,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'UMELCI_UMELECKA_DILA',
        v_idradku,
        v_iduzivatel
    );
END;
/
