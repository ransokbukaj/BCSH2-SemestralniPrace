CREATE OR REPLACE FUNCTION f_vystava_financni_prehled(
    p_idvystava        IN vystavy.idvystava%TYPE,
    p_datum_od         IN DATE,
    p_datum_do         IN DATE,
    p_zahrnout_vstupne IN CHAR DEFAULT 'A',
    p_zahrnout_prodeje IN CHAR DEFAULT 'A'
) RETURN NUMBER
IS
    v_datumod         vystavy.datumod%TYPE;
    v_datumdo         vystavy.datumdo%TYPE;
    v_trzba_vstupne   NUMBER(12,2) := 0;
    v_trzba_prodeje   NUMBER(12,2) := 0;
    v_vysledek        NUMBER(12,2);
BEGIN
    -- načtení dat výstavy
    SELECT datumod, datumdo
    INTO   v_datumod, v_datumdo
    FROM   vystavy
    WHERE  idvystava = p_idvystava;

    IF p_datum_od IS NOT NULL THEN
        v_datumod := p_datum_od;
    END IF;
	
    IF p_datum_do IS NOT NULL THEN
        v_datumdo := p_datum_do;
    END IF;

    IF v_datumod > v_datumdo THEN
        RAISE_APPLICATION_ERROR(-20100, 'Neplatný interval data (od > do).');
    END IF;

    -- tržba ze vstupného
    IF p_zahrnout_vstupne = 'A' THEN
        SELECT NVL(SUM(dn.cena), 0)
        INTO   v_trzba_vstupne
        FROM   navstevy n
               JOIN druhy_navstev dn ON dn.iddruhnavstevy = n.iddruhnavstevy
        WHERE  n.idvystava = p_idvystava
        AND    n.datumnavstevy BETWEEN v_datumod AND v_datumdo;
    END IF;

    -- tržba z prodeje děl
    IF p_zahrnout_prodeje = 'A' THEN
        SELECT NVL(SUM(p.cena), 0)
        INTO   v_trzba_prodeje
        FROM   umelecka_dila d
               JOIN prodeje p ON p.idprodej = d.idprodej
        WHERE  d.idvystava = p_idvystava
        AND    p.datumprodeje BETWEEN v_datumod AND v_datumdo;
    END IF;

    v_vysledek := v_trzba_vstupne + v_trzba_prodeje;
    RETURN v_vysledek;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RAISE_APPLICATION_ERROR(-20101, 'Výstava neexistuje.');
END;
/


CREATE OR REPLACE FUNCTION f_prumerna_cena_prodanych_del_umelce(
    p_idumelec IN umelci.idumelec%TYPE
) RETURN NUMBER
IS
    v_prumer NUMBER(12, 2);
BEGIN
    SELECT NVL(AVG(p.cena), 0)
    INTO   v_prumer
    FROM   umelci_umelecka_dila ud
           JOIN umelecka_dila d ON d.idumeleckedilo = ud.idumeleckedilo
           JOIN prodeje p       ON p.idprodej = d.idprodej
    WHERE  ud.idumelec = p_idumelec;

    RETURN v_prumer;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        -- autor neexistuje nebo nemá žádná prodaná díla
        RETURN 0;
END;
/


CREATE OR REPLACE FUNCTION f_aktualni_status_dila(
    p_idumeleckedilo IN umelecka_dila.idumeleckedilo%TYPE
) RETURN VARCHAR2
IS
    v_nazev_dila     umelecka_dila.nazev%TYPE;
    v_idprodej       umelecka_dila.idprodej%TYPE;
    v_idvystava      umelecka_dila.idvystava%TYPE;
    v_status         VARCHAR2(4000);
    v_kupec_jmeno    VARCHAR2(200);
    v_datumprodeje   DATE;
    v_vystava_nazev  vystavy.nazev%TYPE;
    v_vystava_od     vystavy.datumod%TYPE;
    v_vystava_do     vystavy.datumdo%TYPE;
BEGIN
    -- základní info o díle
    SELECT nazev, idprodej, idvystava
    INTO   v_nazev_dila, v_idprodej, v_idvystava
    FROM   umelecka_dila
    WHERE  idumeleckedilo = p_idumeleckedilo;

    -- Je prodané
    IF v_idprodej IS NOT NULL THEN
        SELECT k.prijmeni || ' ' || k.jmeno,
               p.datumprodeje
        INTO   v_kupec_jmeno,
               v_datumprodeje
        FROM   prodeje p
               JOIN kupci k ON k.idkupec = p.idkupec
        WHERE  p.idprodej = v_idprodej;

        v_status := 'Prodáno kupci: ' || v_kupec_jmeno ||
                    ' dne ' || TO_CHAR(v_datumprodeje, 'DD.MM.YYYY');
        RETURN v_status;
    END IF;

    -- Je na výstavě
    IF v_idvystava IS NOT NULL THEN
        SELECT nazev, datumod, datumdo
        INTO   v_vystava_nazev, v_vystava_od, v_vystava_do
        FROM   vystavy
        WHERE  idvystava = v_idvystava;

        v_status := 'Na výstavě: ' || v_vystava_nazev ||
                    ' (' || TO_CHAR(v_vystava_od, 'DD.MM.YYYY') ||
                    ' - ' || TO_CHAR(v_vystava_do, 'DD.MM.YYYY') || ')';
        RETURN v_status;
    END IF;

    -- Není vystaveno
    v_status := 'V depozitáři (bez výstavy a prodeje)';
    RETURN v_status;

EXCEPTION
    WHEN NO_DATA_FOUND THEN
        RETURN 'Dílo neexistuje.';
END;
/





