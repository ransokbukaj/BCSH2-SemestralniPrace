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

CREATE OR REPLACE VIEW v_navstevy AS
SELECT 
    n.idnavsteva AS id,
    n.datumnavstevy AS datum_navstevy,
    n.iddruhnavstevy AS id_druh_navstevy,
    dn.nazev AS nazev_druhu_navstevy,
    dn.cena AS cena,
    n.idvystava AS id_vystava,
    v.nazev AS nazev_vystavy
FROM navstevy n
    INNER JOIN druhy_navstev dn ON n.iddruhnavstevy = dn.iddruhnavstevy
    INNER JOIN vystavy v ON n.idvystava = v.idvystava
ORDER BY n.datumnavstevy DESC;

CREATE OR REPLACE VIEW v_kupci AS
SELECT 
    k.idkupec AS id,
    k.jmeno AS jmeno,
    k.prijmeni AS prijmeni,
    k.telefonicislo AS telefonni_cislo,
    k.email AS email,
    k.idadresa AS id_adresa,
    a.ulice AS ulice,
    a.cislopopisne AS cislo_popisne,
    a.cisloorientacni AS cislo_orientacni,
    a.idposta AS id_posta,
    p.obec AS obec,
    p.psc AS psc
FROM kupci k
    INNER JOIN adresy a ON k.idadresa = a.idadresa
    INNER JOIN posty p ON a.idposta = p.idposta
ORDER BY k.prijmeni, k.jmeno;

CREATE OR REPLACE VIEW v_prodeje AS
SELECT 
    p.idprodej AS id,
    p.cena AS cena,
    p.datumprodeje AS datum_prodeje,
    p.cislokarty AS cislo_karty,
    p.cislouctu AS cislo_uctu,
    p.iddruhplatby AS id_druh_platby,
    dp.nazev AS nazev_druhu_platby,
    p.idkupec AS id_kupec,
    k.jmeno AS kupec_jmeno,
    k.prijmeni AS kupec_prijmeni,
    k.email AS kupec_email,
    k.telefonicislo AS kupec_telefon
FROM prodeje p
    INNER JOIN druhy_plateb dp ON p.iddruhplatby = dp.iddruhplatby
    INNER JOIN kupci k ON p.idkupec = k.idkupec
ORDER BY p.datumprodeje DESC;

CREATE OR REPLACE VIEW v_zaznamy_historie AS
SELECT 
    zh.idzaznamhistorie AS id,
    zh.datumzmeny AS datum_zmeny,
    zh.popiszmeny AS popis_zmeny,
    zh.druhoperace AS druh_operace,
    zh.starehodnoty AS stare_hodnoty,
    zh.novehodnoty AS nove_hodnoty,
    zh.nazevtabulky AS nazev_tabulky,
    zh.idradkutabulky AS id_radku_tabulky,
    zh.idzmenenohoradku AS id_zmeneneho_radku,
    zh.iduzivatel AS id_uzivatel,
    u.uzivatelskejmeno AS uzivatelske_jmeno,
    u.jmeno AS uzivatel_jmeno,
    u.prijmeni AS uzivatel_prijmeni,
    r.nazev AS nazev_role
FROM zaznamy_historie zh
    INNER JOIN uzivatele u ON zh.iduzivatel = u.iduzivatel
    INNER JOIN role r ON u.idrole = r.idrole
ORDER BY zh.datumzmeny DESC;

CREATE OR REPLACE VIEW v_uzivatele AS
SELECT 
    u.iduzivatel AS id,
    u.uzivatelskejmeno AS uzivatelske_jmeno,
    u.jmeno AS jmeno,
    u.prijmeni AS prijmeni,
    u.email AS email,
    u.telefonicislo AS telefonni_cislo,
    u.datumregistrace AS datum_registrace,
    u.datumposlednihoprihlaseni AS datum_posledniho_prihlaseni,
    u.datumposlednizmeni AS datum_posledni_zmeny,
    u.idrole AS id_role,
    r.nazev AS nazev_role,
    u.deaktivovan AS deaktivovan
FROM uzivatele u
    INNER JOIN role r ON u.idrole = r.idrole
WHERE u.deaktivovan = 0
ORDER BY u.uzivatelskejmeno;