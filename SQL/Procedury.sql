CREATE OR REPLACE PROCEDURE p_hromadne_zvys_ceny_navstev (
    p_procento IN NUMBER
)
IS
BEGIN
    IF p_procento <= -100 THEN
        RAISE_APPLICATION_ERROR(-20020, 'Nelze snížit cenu o 100 % nebo více.');
    END IF;

    UPDATE druhy_navstev
    SET    cena = ROUND(cena * (1 + p_procento / 100), 2);

    DBMS_OUTPUT.PUT_LINE('Počet změněných řádků: ' || SQL%ROWCOUNT);
END;
/




CREATE OR REPLACE PROCEDURE p_prirad_dilo_na_vystavu (
    p_idumeleckedilo IN umelecka_dila.idumeleckedilo%TYPE,
    p_idvystava      IN vystavy.idvystava%TYPE
)
IS
    v_datumzverejneni  umelecka_dila.datumzverejneni%TYPE;
    v_datumod          vystavy.datumod%TYPE;
    v_datumdo          vystavy.datumdo%TYPE;
BEGIN
    SELECT datumzverejneni
    INTO   v_datumzverejneni
    FROM   umelecka_dila
    WHERE  idumeleckedilo = p_idumeleckedilo;

    SELECT datumod, datumdo
    INTO   v_datumod, v_datumdo
    FROM   vystavy
    WHERE  idvystava = p_idvystava;


    IF v_datumzverejneni > v_datumdo THEN
        RAISE_APPLICATION_ERROR(
            -20030,
            'Dílo bylo zveřejněno až po skončení výstavy.'
        );
    END IF;

    UPDATE umelecka_dila
    SET    idvystava = p_idvystava
    WHERE  idumeleckedilo = p_idumeleckedilo;
END;
/



CREATE OR REPLACE PROCEDURE p_zmen_roli_uzivatele(
    p_iduzivatel  IN uzivatele.iduzivatel%TYPE,
    p_idrole_nova IN role.idrole%TYPE,
    p_idadmin     IN uzivatele.iduzivatel%TYPE
)
IS
    v_role_admin_id   role.idrole%TYPE;
    v_je_admin        NUMBER;
    v_stara_role_id   uzivatele.idrole%TYPE;
    v_pocet_adminu    NUMBER;
    v_tabname         CONSTANT VARCHAR2(50) := 'UZIVATELE';
BEGIN
    -- zjistíme ID role ADMIN (název můžeš přizpůsobit)
    SELECT idrole
    INTO   v_role_admin_id
    FROM   role
    WHERE  UPPER(nazev) = 'ADMIN';

    -- ověření, že p_idadmin JE admin
    SELECT COUNT(*)
    INTO   v_je_admin
    FROM   uzivatele u
    WHERE  u.iduzivatel = p_idadmin
    AND    u.idrole = v_role_admin_id;

    IF v_je_admin = 0 THEN
        RAISE_APPLICATION_ERROR(-20130, 'Uživatel nemá oprávnění měnit role.');
    END IF;

    -- načtení staré role měněného uživatele
    SELECT idrole
    INTO   v_stara_role_id
    FROM   uzivatele
    WHERE  iduzivatel = p_iduzivatel
    FOR UPDATE;

    -- kontrola posledního admina
    IF v_stara_role_id = v_role_admin_id
       AND p_idrole_nova <> v_role_admin_id THEN
        SELECT COUNT(*)
        INTO   v_pocet_adminu
        FROM   uzivatele
        WHERE  idrole = v_role_admin_id;

        IF v_pocet_adminu = 1 THEN
            RAISE_APPLICATION_ERROR(-20131, 'Nelze odebrat roli poslednímu administrátorovi.');
        END IF;
    END IF;

    -- aktualizace role
    UPDATE uzivatele
    SET    idrole = p_idrole_nova,
           datumposlednizmeni = SYSDATE
    WHERE  iduzivatel = p_iduzivatel;

   
END;
/

------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE PROCEDURE p_save_adresa(
    p_idadresa IN adresy.idadresa%TYPE,
    p_ulice IN adresy.ulice%TYPE,
    p_cislopopisne IN adresy.cislopopisne%TYPE,
    p_cisloorientacni IN adresy.cisloorientacni%TYPE,
    p_idposta IN adresy.idposta%TYPE
) AS
    v_count NUMBER;
BEGIN
    -- Kontrola, zda adresa s daným ID existuje
    IF p_idadresa IS NOT NULL AND p_idadresa > 0 THEN
        SELECT COUNT(*) INTO v_count
        FROM adresy
        WHERE idadresa = p_idadresa;
        
        IF v_count > 0 THEN
            -- UPDATE - adresa existuje
            UPDATE adresy
            SET ulice = p_ulice,
                cislopopisne = p_cislopopisne,
                cisloorientacni = p_cisloorientacni,
                idposta = p_idposta
            WHERE idadresa = p_idadresa;
        ELSE
            -- ID bylo zadáno, ale záznam neexistuje
            RAISE_APPLICATION_ERROR(-20001, 'Adresa s ID ' || p_idadresa || ' neexistuje.');
        END IF;
    ELSE
        -- INSERT - vytvoření nové adresy
        INSERT INTO adresy (
            ulice,
            cislopopisne,
            cisloorientacni,
            idposta
        ) VALUES (
            p_ulice,
            p_cislopopisne,
            p_cisloorientacni,
            p_idposta
        ) RETURNING idadresa INTO p_idadresa;
    END IF;
END p_save_adresa;
/

CREATE OR REPLACE PROCEDURE p_delete_adresa(
    p_idadresa IN adresy.idadresa%TYPE
) AS
    v_count NUMBER;
BEGIN
    -- Kontrola, zda adresa s daným ID existuje
    SELECT COUNT(*) INTO v_count
    FROM adresy
    WHERE idadresa = p_idadresa;
    
    IF v_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'Adresa s ID ' || p_idadresa || ' neexistuje.');
    END IF;
    
    -- Odstranění adresy
    DELETE FROM adresy
    WHERE idadresa = p_idadresa;    
END p_delete_adresa;
/


CREATE OR REPLACE PROCEDURE p_save_posta(
    p_idposta IN posty.idposta%TYPE,
    p_obec IN posty.obec%TYPE,
    p_psc IN posty.psc%TYPE
) AS
    v_count NUMBER;
BEGIN
    -- Kontrola, zda pošta s daným ID existuje
    IF p_idposta IS NOT NULL AND p_idposta > 0 THEN
        SELECT COUNT(*) INTO v_count
        FROM posty
        WHERE idposta = p_idposta;
        
        IF v_count > 0 THEN
            -- UPDATE - pošta existuje
            UPDATE posty
            SET obec = p_obec,
                psc = p_psc
            WHERE idposta = p_idposta;
        ELSE
            -- ID bylo zadáno, ale záznam neexistuje
            RAISE_APPLICATION_ERROR(-20003, 'Pošta s ID ' || p_idposta || ' neexistuje.');
        END IF;
    ELSE
        -- INSERT - vytvoření nové pošty
        INSERT INTO posty (
            obec,
            psc
        ) VALUES (
            p_obec,
            p_psc
        );
    END IF;
END p_save_posta;
/

CREATE OR REPLACE PROCEDURE p_delete_posta(
    p_idposta IN posty.idposta%TYPE
) AS
    v_count NUMBER;
BEGIN
    -- Kontrola, zda pošta s daným ID existuje
    SELECT COUNT(*) INTO v_count
    FROM posty
    WHERE idposta = p_idposta;
    
    IF v_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20004, 'Pošta s ID ' || p_idposta || ' neexistuje.');
    END IF;
    
    -- Odstranění pošty
    DELETE FROM posty
    WHERE idposta = p_idposta;    
END p_delete_posta;
/


CREATE OR REPLACE PROCEDURE p_save_navsteva(
    p_idnavsteva IN navstevy.idnavsteva%TYPE,
    p_datumnavstevy IN navstevy.datumnavstevy%TYPE,
    p_iddruhnavstevy IN navstevy.iddruhnavstevy%TYPE,
    p_idvystava IN navstevy.idvystava%TYPE
) AS
    v_count NUMBER;
BEGIN
    -- Kontrola, zda návštěva s daným ID existuje
    IF p_idnavsteva IS NOT NULL AND p_idnavsteva > 0 THEN
        SELECT COUNT(*) INTO v_count
        FROM navstevy
        WHERE idnavsteva = p_idnavsteva;
        
        IF v_count > 0 THEN
            -- UPDATE - návštěva existuje
            UPDATE navstevy
            SET datumnavstevy = p_datumnavstevy,
                iddruhnavstevy = p_iddruhnavstevy,
                idvystava = p_idvystava
            WHERE idnavsteva = p_idnavsteva;
        ELSE
            -- ID bylo zadáno, ale záznam neexistuje
            RAISE_APPLICATION_ERROR(-20005, 'Návštěva s ID ' || p_idnavsteva || ' neexistuje.');
        END IF;
    ELSE
        -- INSERT - vytvoření nové návštěvy
        INSERT INTO navstevy (
            datumnavstevy,
            iddruhnavstevy,
            idvystava
        ) VALUES (
            p_datumnavstevy,
            p_iddruhnavstevy,
            p_idvystava
        );
    END IF;
END p_save_navsteva;
/

CREATE OR REPLACE PROCEDURE p_delete_navsteva(
    p_idnavsteva IN navstevy.idnavsteva%TYPE
) AS
    v_count NUMBER;
BEGIN
    -- Kontrola, zda návštěva s daným ID existuje
    SELECT COUNT(*) INTO v_count
    FROM navstevy
    WHERE idnavsteva = p_idnavsteva;
    
    IF v_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20006, 'Návštěva s ID ' || p_idnavsteva || ' neexistuje.');
    END IF;
    
    -- Odstranění návštěvy
    DELETE FROM navstevy
    WHERE idnavsteva = p_idnavsteva;    
END p_delete_navsteva;
/