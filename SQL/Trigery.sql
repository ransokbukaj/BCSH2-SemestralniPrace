CREATE OR REPLACE TRIGGER trg_vystavy_datum_konzistence
BEFORE INSERT OR UPDATE OF datumod, datumdo ON vystavy
FOR EACH ROW
BEGIN
    -- datumdo musí být stejné nebo pozdější než datumod
    IF :NEW.datumdo < :NEW.datumod THEN
        RAISE_APPLICATION_ERROR(
            -20100,
            'Datum konce výstavy nesmí být dříve než datum začátku.'
        );
    END IF;
END;
/

CREATE OR REPLACE TRIGGER trg_vzdelavaci_programy_datum_konzistence
BEFORE INSERT OR UPDATE OF datumod, datumdo ON vzdelavaci_programy
FOR EACH ROW
BEGIN
    -- datumdo musí být stejné nebo pozdější než datumod
    IF :NEW.datumdo < :NEW.datumod THEN
        RAISE_APPLICATION_ERROR(
            -20101,
            'Datum konce vzdělávacího programu nesmí být dříve než datum začátku.'
        );
    END IF;
END;
/

CREATE OR REPLACE TRIGGER trg_prodeje_kontrola_platby
BEFORE INSERT OR UPDATE OF iddruhplatby, cislokarty, cislouctu ON prodeje
FOR EACH ROW
BEGIN
    -- Platba kartou
    IF :NEW.iddruhplatby = 1 THEN
        -- musí být vyplněno číslo karty
        IF :NEW.cislokarty IS NULL THEN
            RAISE_APPLICATION_ERROR(
                -20110,
                'Pro platbu kartou (iddruhplatby = 1) musí být vyplněno číslo karty.'
            );
        END IF;

        -- nesmí být vyplněno číslo účtu
        IF :NEW.cislouctu IS NOT NULL THEN
            RAISE_APPLICATION_ERROR(
                -20111,
                'Pro platbu kartou (iddruhplatby = 1) nesmí být vyplněno číslo účtu.'
            );
        END IF;

    -- Platba převodem na účet
    ELSIF :NEW.iddruhplatby = 2 THEN
        -- musí být vyplněno číslo účtu
        IF :NEW.cislouctu IS NULL THEN
            RAISE_APPLICATION_ERROR(
                -20112,
                'Pro platbu převodem (iddruhplatby = 2) musí být vyplněno číslo účtu.'
            );
        END IF;

        -- nesmí být vyplněno číslo karty
        IF :NEW.cislokarty IS NOT NULL THEN
            RAISE_APPLICATION_ERROR(
                -20113,
                'Pro platbu převodem (iddruhplatby = 2) nesmí být vyplněno číslo karty.'
            );
        END IF;

    -- Jakýkoliv jiný druh platby
    ELSE
        -- ani karta ani účet nesmí být vyplněny
        IF :NEW.cislokarty IS NOT NULL OR :NEW.cislouctu IS NOT NULL THEN
            RAISE_APPLICATION_ERROR(
                -20114,
                'Pro tento druh platby nesmí být vyplněno číslo karty ani číslo účtu.'
            );
        END IF;
    END IF;
END;
/
