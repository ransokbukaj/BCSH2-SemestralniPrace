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
        v_popiszmeny := 'Přidána nová adresa: ' || :NEW.ulice || ' ' || :NEW.cislopopisne;
        
        v_novehodnoty := 
            'ulice: ' || :NEW.ulice || ', ' ||
            'cislopopisne: ' || :NEW.cislopopisne || ', ' ||
            'cisloorientacni: ' || NVL(:NEW.cisloorientacni, 'NULL') || ', ' ||
            'idposta: ' || :NEW.idposta;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idadresa;
        v_popiszmeny := 'Aktualizace adresy: ' || :NEW.ulice || ' ' || :NEW.cislopopisne;
        
        v_starehodnoty := 
            'ulice: ' || :OLD.ulice || ', ' ||
            'cislopopisne: ' || :OLD.cislopopisne || ', ' ||
            'cisloorientacni: ' || NVL(:OLD.cisloorientacni, 'NULL') || ', ' ||
            'idposta: ' || :OLD.idposta;
            
        v_novehodnoty := 
            'ulice: ' || :NEW.ulice || ', ' ||
            'cislopopisne: ' || :NEW.cislopopisne || ', ' ||
            'cisloorientacni: ' || NVL(:NEW.cisloorientacni, 'NULL') || ', ' ||
            'idposta: ' || :NEW.idposta;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idadresa;
        v_popiszmeny := 'Smazána adresa: ' || :OLD.ulice || ' ' || :OLD.cislopopisne;
        
        v_starehodnoty := 
            'ulice: ' || :OLD.ulice || ', ' ||
            'cislopopisne: ' || :OLD.cislopopisne || ', ' ||
            'cisloorientacni: ' || NVL(:OLD.cisloorientacni, 'NULL') || ', ' ||
            'idposta: ' || :OLD.idposta;
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
        idzmenenohoradku,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'ADRESY',
        v_idradku,
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
    
    -- Určení typu operace a ID záznamu
    IF INSERTING THEN
        v_druhoperace := 'INSERT';
        v_idradku := :NEW.iduzivatel;
        v_popiszmeny := 'Přidán nový uživatel: ' || :NEW.uzivatelskejmeno;
        
        v_novehodnoty := 
            'uzivatelskejmeno: ' || :NEW.uzivatelskejmeno || ', ' ||
            'jmeno: ' || :NEW.jmeno || ', ' ||
            'prijmeni: ' || :NEW.prijmeni || ', ' ||
            'email: ' || NVL(:NEW.email, 'NULL') || ', ' ||
            'telefonicislo: ' || NVL(:NEW.telefonicislo, 'NULL') || ', ' ||
            'datumregistrace: ' || TO_CHAR(:NEW.datumregistrace, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'deaktivovan: ' || :NEW.deaktivovan || ', ' ||
            'idrole: ' || :NEW.idrole;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.iduzivatel;
        v_popiszmeny := 'Aktualizace uživatele: ' || :NEW.uzivatelskejmeno;
        
        v_starehodnoty := 
            'uzivatelskejmeno: ' || :OLD.uzivatelskejmeno || ', ' ||
            'jmeno: ' || :OLD.jmeno || ', ' ||
            'prijmeni: ' || :OLD.prijmeni || ', ' ||
            'email: ' || NVL(:OLD.email, 'NULL') || ', ' ||
            'telefonicislo: ' || NVL(:OLD.telefonicislo, 'NULL') || ', ' ||
            'datumregistrace: ' || TO_CHAR(:OLD.datumregistrace, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'datumposlednihoprihlaseni: ' || NVL(TO_CHAR(:OLD.datumposlednihoprihlaseni, 'DD.MM.YYYY HH24:MI:SS'), 'NULL') || ', ' ||
            'datumposlednizmeni: ' || NVL(TO_CHAR(:OLD.datumposlednizmeni, 'DD.MM.YYYY HH24:MI:SS'), 'NULL') || ', ' ||
            'deaktivovan: ' || :OLD.deaktivovan || ', ' ||
            'idrole: ' || :OLD.idrole;
            
        v_novehodnoty := 
            'uzivatelskejmeno: ' || :NEW.uzivatelskejmeno || ', ' ||
            'jmeno: ' || :NEW.jmeno || ', ' ||
            'prijmeni: ' || :NEW.prijmeni || ', ' ||
            'email: ' || NVL(:NEW.email, 'NULL') || ', ' ||
            'telefonicislo: ' || NVL(:NEW.telefonicislo, 'NULL') || ', ' ||
            'datumregistrace: ' || TO_CHAR(:NEW.datumregistrace, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'datumposlednihoprihlaseni: ' || NVL(TO_CHAR(:NEW.datumposlednihoprihlaseni, 'DD.MM.YYYY HH24:MI:SS'), 'NULL') || ', ' ||
            'datumposlednizmeni: ' || NVL(TO_CHAR(:NEW.datumposlednizmeni, 'DD.MM.YYYY HH24:MI:SS'), 'NULL') || ', ' ||
            'deaktivovan: ' || :NEW.deaktivovan || ', ' ||
            'idrole: ' || :NEW.idrole;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.iduzivatel;
        v_popiszmeny := 'Smazán/deaktivován uživatel: ' || :OLD.uzivatelskejmeno;
        
        v_starehodnoty := 
            'uzivatelskejmeno: ' || :OLD.uzivatelskejmeno || ', ' ||
            'jmeno: ' || :OLD.jmeno || ', ' ||
            'prijmeni: ' || :OLD.prijmeni || ', ' ||
            'email: ' || NVL(:OLD.email, 'NULL') || ', ' ||
            'telefonicislo: ' || NVL(:OLD.telefonicislo, 'NULL') || ', ' ||
            'datumregistrace: ' || TO_CHAR(:OLD.datumregistrace, 'DD.MM.YYYY HH24:MI:SS') || ', ' ||
            'datumposlednihoprihlaseni: ' || NVL(TO_CHAR(:OLD.datumposlednihoprihlaseni, 'DD.MM.YYYY HH24:MI:SS'), 'NULL') || ', ' ||
            'datumposlednizmeni: ' || NVL(TO_CHAR(:OLD.datumposlednizmeni, 'DD.MM.YYYY HH24:MI:SS'), 'NULL') || ', ' ||
            'deaktivovan: ' || :OLD.deaktivovan || ', ' ||
            'idrole: ' || :OLD.idrole;
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
        idzmenenohoradku,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'UZIVATELE',
        v_idradku,
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
        idzmenenohoradku,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'POSTY',
        v_idradku,
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
        idzmenenohoradku,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'KUPCI',
        v_idradku,
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
        v_popiszmeny := 'Přidán nový prodej: ' || TO_CHAR(:NEW.cena, '999999999.99') || ' Kč, datum ' || TO_CHAR(:NEW.datumprodeje, 'DD.MM.YYYY');
        
        v_novehodnoty := 
            'cena: ' || TO_CHAR(:NEW.cena, '999999999.99') || ', ' ||
            'datumprodeje: ' || TO_CHAR(:NEW.datumprodeje, 'DD.MM.YYYY') || ', ' ||
            'cislokarty: ' || NVL(:NEW.cislokarty, 'NULL') || ', ' ||
            'cislouctu: ' || NVL(:NEW.cislouctu, 'NULL') || ', ' ||
            'iddruhplatby: ' || :NEW.iddruhplatby || ', ' ||
            'idkupec: ' || :NEW.idkupec;
        v_starehodnoty := NULL;
        
    ELSIF UPDATING THEN
        v_druhoperace := 'UPDATE';
        v_idradku := :NEW.idprodej;
        v_popiszmeny := 'Aktualizace prodeje ID: ' || :NEW.idprodej;
        
        v_starehodnoty := 
            'cena: ' || TO_CHAR(:OLD.cena, '999999999.99') || ', ' ||
            'datumprodeje: ' || TO_CHAR(:OLD.datumprodeje, 'DD.MM.YYYY') || ', ' ||
            'cislokarty: ' || NVL(:OLD.cislokarty, 'NULL') || ', ' ||
            'cislouctu: ' || NVL(:OLD.cislouctu, 'NULL') || ', ' ||
            'iddruhplatby: ' || :OLD.iddruhplatby || ', ' ||
            'idkupec: ' || :OLD.idkupec;
            
        v_novehodnoty := 
            'cena: ' || TO_CHAR(:NEW.cena, '999999999.99') || ', ' ||
            'datumprodeje: ' || TO_CHAR(:NEW.datumprodeje, 'DD.MM.YYYY') || ', ' ||
            'cislokarty: ' || NVL(:NEW.cislokarty, 'NULL') || ', ' ||
            'cislouctu: ' || NVL(:NEW.cislouctu, 'NULL') || ', ' ||
            'iddruhplatby: ' || :NEW.iddruhplatby || ', ' ||
            'idkupec: ' || :NEW.idkupec;
            
    ELSIF DELETING THEN
        v_druhoperace := 'DELETE';
        v_idradku := :OLD.idprodej;
        v_popiszmeny := 'Smazán prodej: ' || TO_CHAR(:OLD.cena, '999999999.99') || ' Kč, datum ' || TO_CHAR(:OLD.datumprodeje, 'DD.MM.YYYY');
        
        v_starehodnoty := 
            'cena: ' || TO_CHAR(:OLD.cena, '999999999.99') || ', ' ||
            'datumprodeje: ' || TO_CHAR(:OLD.datumprodeje, 'DD.MM.YYYY') || ', ' ||
            'cislokarty: ' || NVL(:OLD.cislokarty, 'NULL') || ', ' ||
            'cislouctu: ' || NVL(:OLD.cislouctu, 'NULL') || ', ' ||
            'iddruhplatby: ' || :OLD.iddruhplatby || ', ' ||
            'idkupec: ' || :OLD.idkupec;
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
        idzmenenohoradku,
        iduzivatel
    ) VALUES (
        SYSDATE,
        v_popiszmeny,
        v_druhoperace,
        v_starehodnoty,
        v_novehodnoty,
        'PRODEJE',
        v_idradku,
        v_idradku,
        v_iduzivatel
    );
END;
/