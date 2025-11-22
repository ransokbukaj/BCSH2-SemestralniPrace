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

