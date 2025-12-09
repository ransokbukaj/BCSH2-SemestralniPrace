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
    -- zjistíme ID role Admin
    SELECT idrole
    INTO   v_role_admin_id
    FROM   role
    WHERE  nazev = 'Admin';

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
        );
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



CREATE OR REPLACE PROCEDURE p_save_kupec(
    p_idkupec IN kupci.idkupec%TYPE,
    p_jmeno IN kupci.jmeno%TYPE,
    p_prijmeni IN kupci.prijmeni%TYPE,
    p_telefonicislo IN kupci.telefonicislo%TYPE,
    p_email IN kupci.email%TYPE,
    p_idadresa IN kupci.idadresa%TYPE
) AS
    v_count NUMBER;
    v_adresa_count NUMBER;
BEGIN
    -- Kontrola existence adresy
    SELECT COUNT(*) INTO v_adresa_count
    FROM adresy
    WHERE idadresa = p_idadresa;
    
    IF v_adresa_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20007, 'Adresa s ID ' || p_idadresa || ' neexistuje.');
    END IF;
    
    -- Kontrola, zda kupec s daným ID existuje
    IF p_idkupec IS NOT NULL AND p_idkupec > 0 THEN
        SELECT COUNT(*) INTO v_count
        FROM kupci
        WHERE idkupec = p_idkupec;
        
        IF v_count > 0 THEN
            -- UPDATE - kupec existuje
            UPDATE kupci
            SET jmeno = p_jmeno,
                prijmeni = p_prijmeni,
                telefonicislo = p_telefonicislo,
                email = p_email,
                idadresa = p_idadresa
            WHERE idkupec = p_idkupec;
        ELSE
            -- ID bylo zadáno, ale záznam neexistuje
            RAISE_APPLICATION_ERROR(-20008, 'Kupec s ID ' || p_idkupec || ' neexistuje.');
        END IF;
    ELSE
        -- INSERT - vytvoření nového kupce
        INSERT INTO kupci (
            jmeno,
            prijmeni,
            telefonicislo,
            email,
            idadresa
        ) VALUES (
            p_jmeno,
            p_prijmeni,
            p_telefonicislo,
            p_email,
            p_idadresa
        );
    END IF;
END p_save_kupec;
/

CREATE OR REPLACE PROCEDURE p_delete_kupec(
    p_idkupec IN kupci.idkupec%TYPE
) AS
    v_count NUMBER;
    v_prodeje_count NUMBER;
BEGIN
    -- Kontrola, zda kupec s daným ID existuje
    SELECT COUNT(*) INTO v_count
    FROM kupci
    WHERE idkupec = p_idkupec;
    
    IF v_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20009, 'Kupec s ID ' || p_idkupec || ' neexistuje.');
    END IF;
    
    -- Kontrola, zda kupec nemá nějaké prodeje
    SELECT COUNT(*) INTO v_prodeje_count
    FROM prodeje
    WHERE idkupec = p_idkupec;
    
    IF v_prodeje_count > 0 THEN
        RAISE_APPLICATION_ERROR(-20010, 'Nelze smazat kupce, který má existující prodeje.');
    END IF;
    
    -- Odstranění kupce
    DELETE FROM kupci
    WHERE idkupec = p_idkupec;    
END p_delete_kupec;
/



CREATE OR REPLACE PROCEDURE p_save_prodej(
    p_idprodej IN prodeje.idprodej%TYPE,
    p_cena IN prodeje.cena%TYPE,
    p_datumprodeje IN prodeje.datumprodeje%TYPE,
    p_cislokarty IN prodeje.cislokarty%TYPE,
    p_cislouctu IN prodeje.cislouctu%TYPE,
    p_iddruhplatby IN prodeje.iddruhplatby%TYPE,
    p_idkupec IN prodeje.idkupec%TYPE
) AS
    v_count NUMBER;
    v_kupec_count NUMBER;
    v_druh_platby_count NUMBER;
BEGIN
    -- Kontrola existence kupce
    SELECT COUNT(*) INTO v_kupec_count
    FROM kupci
    WHERE idkupec = p_idkupec;
    
    IF v_kupec_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20011, 'Kupec s ID ' || p_idkupec || ' neexistuje.');
    END IF;
    
    -- Kontrola existence druhu platby
    SELECT COUNT(*) INTO v_druh_platby_count
    FROM druhy_plateb
    WHERE iddruhplatby = p_iddruhplatby;
    
    IF v_druh_platby_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20012, 'Druh platby s ID ' || p_iddruhplatby || ' neexistuje.');
    END IF;
    
    -- Kontrola ceny
    IF p_cena <= 0 THEN
        RAISE_APPLICATION_ERROR(-20013, 'Cena musí být větší než 0.');
    END IF;
    
    -- Kontrola, zda prodej s daným ID existuje
    IF p_idprodej IS NOT NULL AND p_idprodej > 0 THEN
        SELECT COUNT(*) INTO v_count
        FROM prodeje
        WHERE idprodej = p_idprodej;
        
        IF v_count > 0 THEN
            -- UPDATE - prodej existuje
            UPDATE prodeje
            SET cena = p_cena,
                datumprodeje = p_datumprodeje,
                cislokarty = p_cislokarty,
                cislouctu = p_cislouctu,
                iddruhplatby = p_iddruhplatby,
                idkupec = p_idkupec
            WHERE idprodej = p_idprodej;
        ELSE
            -- ID bylo zadáno, ale záznam neexistuje
            RAISE_APPLICATION_ERROR(-20014, 'Prodej s ID ' || p_idprodej || ' neexistuje.');
        END IF;
    ELSE
        -- INSERT - vytvoření nového prodeje
        INSERT INTO prodeje (
            cena,
            datumprodeje,
            cislokarty,
            cislouctu,
            iddruhplatby,
            idkupec
        ) VALUES (
            p_cena,
            p_datumprodeje,
            p_cislokarty,
            p_cislouctu,
            p_iddruhplatby,
            p_idkupec
        );
    END IF;
END p_save_prodej;
/

CREATE OR REPLACE PROCEDURE p_delete_prodej(
    p_idprodej IN prodeje.idprodej%TYPE
) AS
    v_count NUMBER;
    v_dila_count NUMBER;
BEGIN
    -- Kontrola, zda prodej s daným ID existuje
    SELECT COUNT(*) INTO v_count
    FROM prodeje
    WHERE idprodej = p_idprodej;
    
    IF v_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20015, 'Prodej s ID ' || p_idprodej || ' neexistuje.');
    END IF;
    
    -- Kontrola, zda prodej nemá přiřazená umělecká díla
    SELECT COUNT(*) INTO v_dila_count
    FROM umelecka_dila
    WHERE idprodej = p_idprodej;
    
    IF v_dila_count > 0 THEN
        RAISE_APPLICATION_ERROR(-20016, 'Nelze smazat prodej, který má přiřazená umělecká díla.');
    END IF;
    
    -- Odstranění prodeje
    DELETE FROM prodeje
    WHERE idprodej = p_idprodej;    
END p_delete_prodej;
/



CREATE OR REPLACE PROCEDURE p_save_uzivatel(
    p_iduzivatel IN uzivatele.iduzivatel%TYPE,
    p_uzivatelskejmeno IN uzivatele.uzivatelskejmeno%TYPE,
    p_heslohash IN uzivatele.heslohash%TYPE,
    p_jmeno IN uzivatele.jmeno%TYPE,
    p_prijmeni IN uzivatele.prijmeni%TYPE,
    p_email IN uzivatele.email%TYPE,
    p_telefonicislo IN uzivatele.telefonicislo%TYPE,
    p_idrole IN uzivatele.idrole%TYPE
) AS
    v_count NUMBER;
    v_role_count NUMBER;
    v_username_count NUMBER;
BEGIN
    -- Kontrola existence role
    SELECT COUNT(*) INTO v_role_count
    FROM role
    WHERE idrole = p_idrole;
    
    IF v_role_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20017, 'Role s ID ' || p_idrole || ' neexistuje.');
    END IF;
    
    -- Kontrola, zda uživatel s daným ID existuje
    IF p_iduzivatel IS NOT NULL AND p_iduzivatel > 0 THEN
        SELECT COUNT(*) INTO v_count
        FROM uzivatele
        WHERE iduzivatel = p_iduzivatel;
        
        IF v_count > 0 THEN
            -- Kontrola jedinečnosti uživatelského jména
            SELECT COUNT(*) INTO v_username_count
            FROM uzivatele
            WHERE uzivatelskejmeno = p_uzivatelskejmeno
            AND iduzivatel != p_iduzivatel;
            
            IF v_username_count > 0 THEN
                RAISE_APPLICATION_ERROR(-20018, 'Uživatelské jméno "' || p_uzivatelskejmeno || '" je již používáno.');
            END IF;
            
            -- UPDATE - uživatel existuje
            -- Pokud je poskytnuto nové heslo, aktualizuj ho
            IF p_heslohash IS NOT NULL AND LENGTH(p_heslohash) > 0 THEN
                UPDATE uzivatele
                SET uzivatelskejmeno = p_uzivatelskejmeno,
                    heslohash = p_heslohash,
                    jmeno = p_jmeno,
                    prijmeni = p_prijmeni,
                    email = p_email,
                    telefonicislo = p_telefonicislo,
                    idrole = p_idrole,
                    datumposlednizmeni = SYSDATE
                WHERE iduzivatel = p_iduzivatel;
            ELSE
                -- Neaktualizuj heslo, pokud není poskytnuto
                UPDATE uzivatele
                SET uzivatelskejmeno = p_uzivatelskejmeno,
                    jmeno = p_jmeno,
                    prijmeni = p_prijmeni,
                    email = p_email,
                    telefonicislo = p_telefonicislo,
                    idrole = p_idrole,
                    datumposlednizmeni = SYSDATE
                WHERE iduzivatel = p_iduzivatel;
            END IF;
        ELSE
            -- ID bylo zadáno, ale záznam neexistuje
            RAISE_APPLICATION_ERROR(-20019, 'Uživatel s ID ' || p_iduzivatel || ' neexistuje.');
        END IF;
    ELSE
        -- Kontrola jedinečnosti uživatelského jména pro nového uživatele
        SELECT COUNT(*) INTO v_username_count
        FROM uzivatele
        WHERE uzivatelskejmeno = p_uzivatelskejmeno;
        
        IF v_username_count > 0 THEN
            RAISE_APPLICATION_ERROR(-20018, 'Uživatelské jméno "' || p_uzivatelskejmeno || '" je již používáno.');
        END IF;
        
        -- Kontrola, že heslo je poskytnuto pro nového uživatele
        IF p_heslohash IS NULL OR LENGTH(p_heslohash) = 0 THEN
            RAISE_APPLICATION_ERROR(-20020, 'Pro nového uživatele musí být zadáno heslo.');
        END IF;
        
        -- INSERT - vytvoření nového uživatele
        INSERT INTO uzivatele (
            uzivatelskejmeno,
            heslohash,
            jmeno,
            prijmeni,
            email,
            telefonicislo,
            datumregistrace,
            deaktivovan,
            idrole
        ) VALUES (
            p_uzivatelskejmeno,
            p_heslohash,
            p_jmeno,
            p_prijmeni,
            p_email,
            p_telefonicislo,
            SYSDATE,
            0,
            p_idrole
        );
    END IF;
END p_save_uzivatel;
/

CREATE OR REPLACE PROCEDURE p_delete_uzivatel(
    p_iduzivatel IN uzivatele.iduzivatel%TYPE
) AS
    v_count NUMBER;
    v_historie_count NUMBER;
    v_is_admin NUMBER;
    v_admin_count NUMBER;
    v_role_admin_id role.idrole%TYPE;
BEGIN
    -- Kontrola, zda uživatel s daným ID existuje
    SELECT COUNT(*) INTO v_count
    FROM uzivatele
    WHERE iduzivatel = p_iduzivatel;
    
    IF v_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20021, 'Uživatel s ID ' || p_iduzivatel || ' neexistuje.');
    END IF;
    
    -- Zjistíme ID role Admin
    SELECT idrole INTO v_role_admin_id
    FROM role
    WHERE nazev = 'Admin';
    
    -- Kontrola, zda uživatel není poslední aktivní admin
    SELECT COUNT(*) INTO v_is_admin
    FROM uzivatele
    WHERE iduzivatel = p_iduzivatel
    AND idrole = v_role_admin_id;
    
    IF v_is_admin > 0 THEN
        -- Uživatel je admin, zkontrolujeme, zda není poslední aktivní
        SELECT COUNT(*) INTO v_admin_count
        FROM uzivatele
        WHERE idrole = v_role_admin_id
        AND deaktivovan = 0;
        
        IF v_admin_count = 1 THEN
            RAISE_APPLICATION_ERROR(-20022, 'Nelze smazat posledního aktivního administrátora.');
        END IF;
    END IF;
    
    -- Kontrola, zda uživatel má záznamy v historii
    SELECT COUNT(*) INTO v_historie_count
    FROM zaznamy_historie
    WHERE iduzivatel = p_iduzivatel;
    
    IF v_historie_count > 0 THEN
        -- Uživatel má záznamy v historii - pouze deaktivujeme
        UPDATE uzivatele
        SET deaktivovan = 1,
            datumposlednizmeni = SYSDATE
        WHERE iduzivatel = p_iduzivatel;
    ELSE
        -- Uživatel nemá záznamy v historii - můžeme fyzicky smazat
        DELETE FROM uzivatele
        WHERE iduzivatel = p_iduzivatel;
    END IF;
END p_delete_uzivatel;
/

CREATE OR REPLACE PROCEDURE p_change_password(
    p_iduzivatel IN uzivatele.iduzivatel%TYPE,
    p_noveheslohash IN uzivatele.heslohash%TYPE
) AS
    v_count NUMBER;
BEGIN
    -- Kontrola, zda uživatel existuje
    SELECT COUNT(*) INTO v_count
    FROM uzivatele
    WHERE iduzivatel = p_iduzivatel;
    
    IF v_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20024, 'Uživatel s ID ' || p_iduzivatel || ' neexistuje.');
    END IF;
    
    -- Kontrola, že nové heslo není prázdné
    IF p_noveheslohash IS NULL OR LENGTH(p_noveheslohash) = 0 THEN
        RAISE_APPLICATION_ERROR(-20025, 'Nové heslo nesmí být prázdné.');
    END IF;
    
    -- Aktualizace hesla
    UPDATE uzivatele
    SET heslohash = p_noveheslohash,
        datumposlednizmeni = SYSDATE
    WHERE iduzivatel = p_iduzivatel;
END p_change_password;
/



CREATE OR REPLACE PROCEDURE p_save_obraz(
    p_idumeleckedilo IN umelecka_dila.idumeleckedilo%TYPE,
    p_nazev IN umelecka_dila.nazev%TYPE,
    p_popis IN umelecka_dila.popis%TYPE,
    p_datumzverejneni IN umelecka_dila.datumzverejneni%TYPE,
    p_vyska IN umelecka_dila.vyska%TYPE,
    p_sirka IN umelecka_dila.sirka%TYPE,
    p_idprodej IN umelecka_dila.idprodej%TYPE,
    p_idvystava IN umelecka_dila.idvystava%TYPE,
    p_idpodklad IN obrazy.idpodklad%TYPE,
    p_idtechnika IN obrazy.idtechnika%TYPE
) AS
    v_count NUMBER;
    v_podklad_count NUMBER;
    v_technika_count NUMBER;
    v_prodej_count NUMBER;
    v_vystava_count NUMBER;
    v_new_id umelecka_dila.idumeleckedilo%TYPE;
BEGIN
    -- Kontrola existence podkladu
    SELECT COUNT(*) INTO v_podklad_count
    FROM podklady
    WHERE idpodklad = p_idpodklad;
    
    IF v_podklad_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20050, 'Podklad s ID ' || p_idpodklad || ' neexistuje.');
    END IF;
    
    -- Kontrola existence techniky
    SELECT COUNT(*) INTO v_technika_count
    FROM techniky
    WHERE idtechnika = p_idtechnika;
    
    IF v_technika_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20051, 'Technika s ID ' || p_idtechnika || ' neexistuje.');
    END IF;
    
    -- Kontrola existence prodeje (pokud je zadán)
    IF p_idprodej IS NOT NULL THEN
        SELECT COUNT(*) INTO v_prodej_count
        FROM prodeje
        WHERE idprodej = p_idprodej;
        
        IF v_prodej_count = 0 THEN
            RAISE_APPLICATION_ERROR(-20052, 'Prodej s ID ' || p_idprodej || ' neexistuje.');
        END IF;
    END IF;
    
    -- Kontrola existence výstavy (pokud je zadána)
    IF p_idvystava IS NOT NULL THEN
        SELECT COUNT(*) INTO v_vystava_count
        FROM vystavy
        WHERE idvystava = p_idvystava;
        
        IF v_vystava_count = 0 THEN
            RAISE_APPLICATION_ERROR(-20053, 'Výstava s ID ' || p_idvystava || ' neexistuje.');
        END IF;
    END IF;
    
    -- Kontrola, že výška a šířka jsou kladné
    IF p_vyska <= 0 OR p_sirka <= 0 THEN
        RAISE_APPLICATION_ERROR(-20054, 'Výška a šířka musí být větší než 0.');
    END IF;
    
    -- Kontrola, zda obraz s daným ID existuje
    IF p_idumeleckedilo IS NOT NULL AND p_idumeleckedilo > 0 THEN
        SELECT COUNT(*) INTO v_count
        FROM umelecka_dila
        WHERE idumeleckedilo = p_idumeleckedilo;
        
        IF v_count > 0 THEN
            -- UPDATE - obraz existuje
            UPDATE umelecka_dila
            SET nazev = p_nazev,
                popis = p_popis,
                datumzverejneni = p_datumzverejneni,
                vyska = p_vyska,
                sirka = p_sirka,
                idprodej = p_idprodej,
                idvystava = p_idvystava
            WHERE idumeleckedilo = p_idumeleckedilo;
            
            -- Aktualizace specifických dat obrazu
            UPDATE obrazy
            SET idpodklad = p_idpodklad,
                idtechnika = p_idtechnika
            WHERE idumeleckedilo = p_idumeleckedilo;
        ELSE
            -- ID bylo zadáno, ale záznam neexistuje
            RAISE_APPLICATION_ERROR(-20055, 'Obraz s ID ' || p_idumeleckedilo || ' neexistuje.');
        END IF;
    ELSE
        -- INSERT - vytvoření nového obrazu
        INSERT INTO umelecka_dila (
            nazev,
            popis,
            datumzverejneni,
            vyska,
            sirka,
            idprodej,
            idvystava,
            typdila
        ) VALUES (
            p_nazev,
            p_popis,
            p_datumzverejneni,
            p_vyska,
            p_sirka,
            p_idprodej,
            p_idvystava,
            'O'
        ) RETURNING idumeleckedilo INTO v_new_id;
        
        -- Vložení specifických dat obrazu
        INSERT INTO obrazy (
            idumeleckedilo,
            idpodklad,
            idtechnika
        ) VALUES (
            v_new_id,
            p_idpodklad,
            p_idtechnika
        );
    END IF;
END p_save_obraz;
/

CREATE OR REPLACE PROCEDURE p_delete_obraz(
    p_idumeleckedilo IN umelecka_dila.idumeleckedilo%TYPE
) AS
    v_count NUMBER;
    v_prilohy_count NUMBER;
BEGIN
    -- Kontrola, zda obraz s daným ID existuje
    SELECT COUNT(*) INTO v_count
    FROM umelecka_dila
    WHERE idumeleckedilo = p_idumeleckedilo
    AND typdila = 'O';
    
    IF v_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20056, 'Obraz s ID ' || p_idumeleckedilo || ' neexistuje.');
    END IF;
    
    -- Kontrola, zda obraz má přílohy
    SELECT COUNT(*) INTO v_prilohy_count
    FROM prilohy
    WHERE idumeleckedilo = p_idumeleckedilo;
    
    IF v_prilohy_count > 0 THEN
        -- Smazání příloh
        DELETE FROM prilohy
        WHERE idumeleckedilo = p_idumeleckedilo;
    END IF;
    
    -- Smazání specifických dat obrazu
    DELETE FROM obrazy
    WHERE idumeleckedilo = p_idumeleckedilo;
    
    -- Smazání obecných dat uměleckého díla
    DELETE FROM umelecka_dila
    WHERE idumeleckedilo = p_idumeleckedilo;
END p_delete_obraz;
/



CREATE OR REPLACE PROCEDURE p_save_socha(
    p_idumeleckedilo IN umelecka_dila.idumeleckedilo%TYPE,
    p_nazev IN umelecka_dila.nazev%TYPE,
    p_popis IN umelecka_dila.popis%TYPE,
    p_datumzverejneni IN umelecka_dila.datumzverejneni%TYPE,
    p_vyska IN umelecka_dila.vyska%TYPE,
    p_sirka IN umelecka_dila.sirka%TYPE,
    p_idprodej IN umelecka_dila.idprodej%TYPE,
    p_idvystava IN umelecka_dila.idvystava%TYPE,
    p_hloubka IN sochy.hloubka%TYPE,
    p_hmotnost IN sochy.hmotnost%TYPE,
    p_idmaterial IN sochy.idmaterial%TYPE
) AS
    v_count NUMBER;
    v_material_count NUMBER;
    v_prodej_count NUMBER;
    v_vystava_count NUMBER;
    v_new_id umelecka_dila.idumeleckedilo%TYPE;
BEGIN
    -- Kontrola existence materiálu
    SELECT COUNT(*) INTO v_material_count
    FROM materialy
    WHERE idmaterial = p_idmaterial;
    
    IF v_material_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20060, 'Materiál s ID ' || p_idmaterial || ' neexistuje.');
    END IF;
    
    -- Kontrola existence prodeje (pokud je zadán)
    IF p_idprodej IS NOT NULL THEN
        SELECT COUNT(*) INTO v_prodej_count
        FROM prodeje
        WHERE idprodej = p_idprodej;
        
        IF v_prodej_count = 0 THEN
            RAISE_APPLICATION_ERROR(-20061, 'Prodej s ID ' || p_idprodej || ' neexistuje.');
        END IF;
    END IF;
    
    -- Kontrola existence výstavy (pokud je zadána)
    IF p_idvystava IS NOT NULL THEN
        SELECT COUNT(*) INTO v_vystava_count
        FROM vystavy
        WHERE idvystava = p_idvystava;
        
        IF v_vystava_count = 0 THEN
            RAISE_APPLICATION_ERROR(-20062, 'Výstava s ID ' || p_idvystava || ' neexistuje.');
        END IF;
    END IF;
    
    -- Kontrola, že rozměry jsou kladné
    IF p_vyska <= 0 OR p_sirka <= 0 OR p_hloubka <= 0 THEN
        RAISE_APPLICATION_ERROR(-20063, 'Výška, šířka a hloubka musí být větší než 0.');
    END IF;
    
    -- Kontrola, že hmotnost je kladná
    IF p_hmotnost <= 0 THEN
        RAISE_APPLICATION_ERROR(-20064, 'Hmotnost musí být větší než 0.');
    END IF;
    
    -- Kontrola, zda socha s daným ID existuje
    IF p_idumeleckedilo IS NOT NULL AND p_idumeleckedilo > 0 THEN
        SELECT COUNT(*) INTO v_count
        FROM umelecka_dila
        WHERE idumeleckedilo = p_idumeleckedilo;
        
        IF v_count > 0 THEN
            -- UPDATE - socha existuje
            UPDATE umelecka_dila
            SET nazev = p_nazev,
                popis = p_popis,
                datumzverejneni = p_datumzverejneni,
                vyska = p_vyska,
                sirka = p_sirka,
                idprodej = p_idprodej,
                idvystava = p_idvystava
            WHERE idumeleckedilo = p_idumeleckedilo;
            
            -- Aktualizace specifických dat sochy
            UPDATE sochy
            SET hloubka = p_hloubka,
                hmotnost = p_hmotnost,
                idmaterial = p_idmaterial
            WHERE idumeleckedilo = p_idumeleckedilo;
        ELSE
            -- ID bylo zadáno, ale záznam neexistuje
            RAISE_APPLICATION_ERROR(-20065, 'Socha s ID ' || p_idumeleckedilo || ' neexistuje.');
        END IF;
    ELSE
        -- INSERT - vytvoření nové sochy
        INSERT INTO umelecka_dila (
            nazev,
            popis,
            datumzverejneni,
            vyska,
            sirka,
            idprodej,
            idvystava,
            typdila
        ) VALUES (
            p_nazev,
            p_popis,
            p_datumzverejneni,
            p_vyska,
            p_sirka,
            p_idprodej,
            p_idvystava,
            'S'
        ) RETURNING idumeleckedilo INTO v_new_id;
        
        -- Vložení specifických dat sochy
        INSERT INTO sochy (
            idumeleckedilo,
            hloubka,
            hmotnost,
            idmaterial
        ) VALUES (
            v_new_id,
            p_hloubka,
            p_hmotnost,
            p_idmaterial
        );
    END IF;
END p_save_socha;
/

CREATE OR REPLACE PROCEDURE p_delete_socha(
    p_idumeleckedilo IN umelecka_dila.idumeleckedilo%TYPE
) AS
    v_count NUMBER;
    v_prilohy_count NUMBER;
BEGIN
    -- Kontrola, zda socha s daným ID existuje
    SELECT COUNT(*) INTO v_count
    FROM umelecka_dila
    WHERE idumeleckedilo = p_idumeleckedilo
    AND typdila = 'S';
    
    IF v_count = 0 THEN
        RAISE_APPLICATION_ERROR(-20066, 'Socha s ID ' || p_idumeleckedilo || ' neexistuje.');
    END IF;
    
    -- Kontrola, zda socha má přílohy
    SELECT COUNT(*) INTO v_prilohy_count
    FROM prilohy
    WHERE idumeleckedilo = p_idumeleckedilo;
    
    IF v_prilohy_count > 0 THEN
        -- Smazání příloh
        DELETE FROM prilohy
        WHERE idumeleckedilo = p_idumeleckedilo;
    END IF;
    
    -- Smazání specifických dat sochy
    DELETE FROM sochy
    WHERE idumeleckedilo = p_idumeleckedilo;
    
    -- Smazání obecných dat uměleckého díla
    DELETE FROM umelecka_dila
    WHERE idumeleckedilo = p_idumeleckedilo;
END p_delete_socha;
/