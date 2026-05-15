CREATE TABLE IF NOT EXISTS Facturas
(
    NumeroDocumento     INTEGER PRIMARY KEY,
    RutVendedor         INTEGER NOT NULL,
    DvVendedor          TEXT NOT NULL,
    RutComprador        INTEGER NOT NULL,
    DvComprador         TEXT NOT NULL,
    DireccionComprador  TEXT NOT NULL,
    ComunaComprador     INTEGER NOT NULL,
    RegionComprador     INTEGER NOT NULL,
    TotalFactura        REAL NOT NULL
);

CREATE TABLE IF NOT EXISTS DetalleFactura
(
    Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
    NumeroDocumento     INTEGER NOT NULL,
    CantidadProducto    REAL NOT NULL,
    DescripcionProducto TEXT NOT NULL,
    ValorProducto       REAL NOT NULL,
    TotalProducto       REAL NOT NULL,

    FOREIGN KEY (NumeroDocumento)
        REFERENCES Facturas(NumeroDocumento)
);