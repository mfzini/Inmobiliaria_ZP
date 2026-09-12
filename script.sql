SET GLOBAL time_zone = '-03:00';
create database if not exists inmobiliaria_pz;

use inmobiliaria_pz;

create table if not exists Personas(
    dni varchar(64) primary key,
    apellido varchar(64) not null,
    nombre varchar(64) not null,
    email varchar(64) unique not null,
    telefono varchar(64) default "_"
);

create table if not exists TipoInmueble(
	id int auto_increment primary key,
	nombre varchar(20) not null
);

create table if not exists Inmuebles(
    id varchar(36) primary key default (uuid()),
    propietario varchar(64) not null,
    tipo int not null,
    constraint fk_inmbueble_tipo foreign key (tipo)
        references TipoInmueble(id)
        ON UPDATE CASCADE,
    direccion varchar(64) not null,
    latitud decimal(9,6) default 0,
    longitud decimal(9,6) default 0,
    capacidad tinyint not null default 1,
    precio decimal(10,2) default 0,
    porcentaje_reserva decimal(10,2) not null default 0,
    portada varchar(250) default null,
    listado boolean default false,
    constraint fk_inmbueble_propietario foreign key (propietario)
        references Personas(dni)
        ON UPDATE CASCADE
);

create table if not exists ImagenesInmuebles(
    id varchar(36) primary key default (uuid()),
    inmueble varchar(36) not null,
    original_name varchar(250) not null,
    location varchar(250) not null,
    is_portada boolean default 0,
    constraint fk_imagen_inmuebles foreign key (inmueble)
        references Inmuebles(id)
);

create table if not exists Reservas(
    id varchar(36) primary key default (uuid()),
    inmueble varchar(36) not null,
    constraint fk_reserva_inmueble foreign key (inmueble)
        references Inmuebles(id),
    inquilino varchar(64) not null,
    constraint fk_reserva_inquilino foreign key (inquilino)
        references Personas(dni)
        ON UPDATE CASCADE,
    fecha_inicio date not null,
    fecha_fin date not null
);
create table if not exists ConceptoPago(
    id int auto_increment primary key,
    nombre varchar(20) not null
);

create table if not exists Pagos(
    id varchar(36) primary key default (uuid()),
    reserva varchar(36) not null,
    constraint fk_pago_reserva foreign key (reserva)
        references Reservas(id),
    monto decimal(10,2) not null,
    concepto int not null,
    constraint fk_concepto_pago foreign key (concepto)
        references ConceptoPago(id),
    fecha timestamp default now(),
    anulado boolean default 0
    
);


create table if not exists Usuario(
    dni varchar(64) primary key,
    apellido varchar(64) not null,
    nombre varchar(64) not null,
    email varchar(64) unique not null,
    telefono varchar(64) default "_",
    password varchar(64) not null,
    rol varchar(64) not null,
    urlAvatar varchar(255) not null default
);