SET GLOBAL time_zone = '-03:00';
create database if not exists inmobiliaria_pz;

use inmobiliaria_pz;

create table if not exists Personas(
    dni int primary key,
    apellido varchar(64) not null,
    nombre varchar(64) not null,
    email varchar(64) unique not null,
    telefono varchar(64)
);

create table if not exists Inmuebles(
    id varchar(36) primary key default (uuid()),
    propietario int not null,
    tipo varchar(64) not null,
    direccion varchar(64) not null,
    latitud decimal(9,6),
    longitud decimal(9,6),
    capacidad tinyint not null,
    precio decimal(10,2) default 0,
    listado boolean default false,
    constraint fk_inmbueble_propietario foreign key (propietario)
        references Personas(dni)
);

create table if not exists Reservas(
    id varchar(36) primary key default (uuid()),
    inmueble varchar(36) not null,
    constraint fk_reserva_inmueble foreign key (inmueble)
        references Inmuebles(id),
    inquilino int not null,
    constraint fk_reserva_inquilino foreign key (inquilino)
        references Personas(dni),
    fecha_inicio date not null,
    fecha_fin date not null
);

create table if not exists Pagos(
    id varchar(36) primary key default (uuid()),
    reserva varchar(36) not null,
    constraint fk_pago_reserva foreign key (reserva)
        references Reservas(id),
    monto decimal(10,2) not null,
    concepto varchar(64) not null,
    fecha timestamp default now()
);