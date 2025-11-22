CREATE OR REPLACE TRIGGER trg_navstevy_kontrola_intervalu
    BEFORE INSERT OR UPDATE OF datumnavstevy, idvystava
    ON navstevy
    FOR EACH ROW
DECLARE
    v_datumod  vystavy.datumod%TYPE;
    v_datumdo  vystavy.datumdo%TYPE;
BEGIN
    -- načteme interval výstavy
    SELECT datumod, datumdo
    INTO   v_datumod, v_datumdo
    FROM   vystavy
    WHERE  idvystava = :NEW.idvystava;

    -- kontrola intervalu
    IF :NEW.datumnavstevy < v_datumod
       OR :NEW.datumnavstevy > v_datumdo THEN
        RAISE_APPLICATION_ERROR(
            -21000,
            'Datum návštěvy ('
            || TO_CHAR(:NEW.datumnavstevy, ''DD.MM.YYYY'')
            || ') není v intervalu výstavy ('
            || TO_CHAR(v_datumod, ''DD.MM.YYYY'')
            || ' - '
            || TO_CHAR(v_datumdo, ''DD.MM.YYYY'')
            || ').'
        );
    END IF;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-21001, 'Výstava s daným ID neexistuje.');
END;
/



CREATE OR REPLACE TRIGGER trg_prodeje_kontrola_platby
    BEFORE INSERT OR UPDATE
    ON prodeje
    FOR EACH ROW
DECLARE
    v_nazev_druhu  druhy_plateb.nazev%TYPE;
BEGIN
    -- načteme název druhu platby
    SELECT nazev
    INTO   v_nazev_druhu
    FROM   druhy_plateb
    WHERE  iddruhplatby = :NEW.iddruhplatby;


    IF :NEW.cislokarty IS NULL AND :NEW.cislouctu IS NULL THEN
        RAISE_APPLICATION_ERROR(
            -21010,
            'Musí být zadáno číslo karty nebo číslo účtu.'
        );
    END IF;

    IF :NEW.cislokarty IS NOT NULL AND :NEW.cislouctu IS NOT NULL THEN
        RAISE_APPLICATION_ERROR(
            -21011,
            'Nelze zadat současně číslo karty i číslo účtu.'
        );
    END IF;

    -- logika podle typu platby
    IF UPPER(v_nazev_druhu) LIKE '%KARTA%' THEN

        IF :NEW.cislokarty IS NULL THEN
            RAISE_APPLICATION_ERROR(
                -21012,
                'Pro platbu kartou musí být zadáno číslo karty.'
            );
        END IF;
        IF :NEW.cislouctu IS NOT NULL THEN
            RAISE_APPLICATION_ERROR(
                -21013,
                'Pro platbu kartou nesmí být zadáno číslo účtu.'
            );
        END IF;

        -- kontrola formátu karty
        IF LENGTH(:NEW.cislokarty) < 12 OR LENGTH(:NEW.cislokarty) > 19 THEN
            RAISE_APPLICATION_ERROR(
                -21014,
                'Číslo karty musí mít 12 až 19 znaků.'
            );
        END IF;

        IF NOT REGEXP_LIKE(:NEW.cislokarty, '^[0-9]+$') THEN
            RAISE_APPLICATION_ERROR(
                -21015,
                'Číslo karty může obsahovat pouze číslice.'
            );
        END IF;
    ELSIF UPPER(v_nazev_druhu) LIKE '%PREVOD%' THEN
        -- bankovní převod: má být číslo účtu
        IF :NEW.cislouctu IS NULL THEN
            RAISE_APPLICATION_ERROR(
                -21016,
                'Pro platbu převodem musí být zadáno číslo účtu.'
            );
        END IF;
    END IF;
END;
/



