CREATE OR REPLACE VIEW v_umelecka_dila_prehled AS
SELECT
    d.idumeleckedilo,
    d.nazev  AS dilo_nazev,
    d.typdila,
    d.datumzverejneni,
    v.nazev AS vystava_nazev,
    p.cena  AS prodejni_cena,
    LISTAGG(u.prijmeni || ' ' || u.jmeno, ', ')
        WITHIN GROUP (ORDER BY u.prijmeni, u.jmeno) AS umelci
FROM umelecka_dila d
    LEFT JOIN vystavy v ON v.idvystava = d.idvystava
    LEFT JOIN prodeje p ON p.idprodej = d.idprodej
    LEFT JOIN umelci_umelecka_dila ud ON ud.idumeleckedilo = d.idumeleckedilo
    LEFT JOIN umelci u ON u.idumelec = ud.idumelec
GROUP BY
    d.idumeleckedilo, d.nazev, d.typdila, d.datumzverejneni,
    v.nazev, p.cena;


CREATE OR REPLACE VIEW v_prodeje_prehled AS
SELECT
    pr.idprodej,
    d.nazev AS dilo_nazev,
    pr.cena,
    pr.datumprodeje,
    dp.nazev AS druh_platby,
    k.jmeno || ' ' || k.prijmeni AS kupec_jmeno,
    k.telefonicislo,
    k.email,
    a.ulice
      || ' ' || a.cislopopisne
      || NVL2(a.cisloorientacni, '/' || a.cisloorientacni, '') AS kupec_adresa,
    po.psc,
    po.obec
FROM prodeje pr
    JOIN umelecka_dila d ON d.idprodej = pr.idprodej
    JOIN druhy_plateb dp ON dp.iddruhplatby = pr.iddruhplatby
    JOIN kupci k ON k.idkupec = pr.idkupec
    JOIN adresy a ON a.idadresa = k.idadresa
    JOIN posty po ON po.idposta = a.idposta;


CREATE OR REPLACE VIEW v_navstevy_statistika AS
SELECT
    v.nazev AS vystava_nazev,
    n.datumnavstevy,
    dn.nazev AS druh_navstevy,
    COUNT(*) AS pocet
FROM navstevy n
    JOIN druhy_navstev dn ON dn.iddruhnavstevy = n.iddruhnavstevy
    JOIN vystavy v ON v.idvystava = n.idvystava
GROUP BY v.nazev, n.datumnavstevy, dn.nazev;

------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE VIEW v_adresy AS
SELECT 
    a.idadresa AS id,
    a.ulice AS ulice,
    a.cislopopisne AS cislo_popisne,
    a.cisloorientacni AS cislo_orientacni,
    a.idposta AS id_posta,
    p.obec AS obec,
    p.psc AS psc
FROM adresy a
INNER JOIN posty p ON a.idposta = p.idposta
ORDER BY a.idadresa;

CREATE OR REPLACE VIEW v_posty AS
SELECT 
    p.idposta AS id,
    p.obec AS obec,
    p.psc AS psc
FROM posty p
ORDER BY p.obec;
