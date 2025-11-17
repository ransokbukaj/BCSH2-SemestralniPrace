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