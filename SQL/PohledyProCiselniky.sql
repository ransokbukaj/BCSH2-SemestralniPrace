-- Pohled pro druhy návštěv
CREATE OR REPLACE VIEW v_druhy_navstev AS
SELECT 
    iddruhnavstevy AS id,
    nazev,
    cena
FROM druhy_navstev
ORDER BY nazev;

-- Pohled pro druhy plateb
CREATE OR REPLACE VIEW v_druhy_plateb AS
SELECT 
    iddruhplatby AS id,
    nazev
FROM druhy_plateb
ORDER BY nazev;

-- Pohled pro materiály
CREATE OR REPLACE VIEW v_materialy AS
SELECT 
    idmaterial AS id,
    nazev
FROM materialy
ORDER BY nazev;

-- Pohled pro podklady
CREATE OR REPLACE VIEW v_podklady AS
SELECT 
    idpodklad AS id,
    nazev
FROM podklady
ORDER BY nazev;

-- Pohled pro role
CREATE OR REPLACE VIEW v_role AS
SELECT 
    idrole AS id,
    nazev
FROM role
ORDER BY nazev;

-- Pohled pro techniky
CREATE OR REPLACE VIEW v_techniky AS
SELECT 
    idtechnika AS id,
    nazev
FROM techniky
ORDER BY nazev;

-- Pohled pro zjednodušené výstavy jako číselníky
CREATE OR REPLACE VIEW v_vystavy_jako_ciselniky AS
SELECT
    idvystava AS id,
    nazev
FROM vystavy
ORDER BY nazev;

-- Pohled pro zjednodušená umělecká díla jako číselníky
CREATE OR REPLACE VIEW v_umelecka_dila_jako_ciselniky AS
SELECT
    idumeleckedilo AS id,
    nazev
FROM umelecka_dila
ORDER BY nazev;