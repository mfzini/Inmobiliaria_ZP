-- cat script.sql | podman exec -i sandbox_db_1 mariadb -u root -proot
SET GLOBAL time_zone = '-03:00';
create database if not exists inmobiliaria;

use inmobiliaria;

create table if not exists Personas(
	dni int primary key,
	apellido tinytext not null,
	nombre tinytext not null,
	email tinytext unique not null,
	telefono tinytext
);

create table if not exists Inmuebles(
	id uuid primary key default uuid(),
	propietario int not null,
	tipo tinytext not null,
	direccion tinytext not null,
	latitud decimal(9,6),
	longitud decimal(9,6),
	capacidad tinyint not null,
	precio decimal(10,2) default 0,
	listado boolean default false,
	constraint fk_inmbueble_propietario foreign key (propietario)
		references Personas(dni)
);

create table if not exists Reservas(
	id uuid primary key default uuid(),
	inmueble uuid not null,
	constraint fk_reserva_inmueble foreign key (inmueble)
		references Inmuebles(id),
	inquilino int not null,
	constraint fk_reserva_inquilino foreign key (inquilino)
		references Personas(dni),
	fecha_inicio date not null,
	fecha_fin date not null
);

create table if not exists Pagos(
	id uuid primary key default uuid(),
	reserva uuid not null,
    constraint fk_pago_reserva foreign key (reserva)
        references Reservas(id),
	monto decimal(10,2) not null,
    concepto tinytext not null,
	fecha timestamp default now()
);

create table if not exists Users(
	dni int primary key,
	constraint fk_user_persona foreign key (dni)
		references Personas(dni),
	role tinytext,
	password tinytext not null,
	avatar tinytext not null
);