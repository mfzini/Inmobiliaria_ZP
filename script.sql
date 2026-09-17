SET GLOBAL time_zone = '-03:00';
-- drop database if exists inmobiliaria_pz;
create database if not exists inmobiliaria_pz;

use inmobiliaria_pz;

create table if not exists Personas(
    dni varchar(64) primary key,
    apellido varchar(64) not null,
    nombre varchar(64) not null,
    email varchar(64) unique not null,
    telefono varchar(64) default '_'
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
    monto decimal(10,2) default 0,
    constraint fk_reserva_inmueble foreign key (inmueble)
        references Inmuebles(id),
    inquilino varchar(64) not null,
    constraint fk_reserva_inquilino foreign key (inquilino)
        references Personas(dni)
        ON UPDATE CASCADE,
    fecha_inicio date not null,
    fecha_fin date not null,
    fecha_cancelacion date default null
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

create table if not exists Usuarios(
    dni varchar(64) primary key,
    constraint fk_usuario_persona foreign key (dni)
        references Personas(dni),
    password varchar(250) not null,
    role varchar(64) not null,
    avatar varchar(255) not null default ''
);

create table if not exists Logs(
    dni varchar(64) not null,
    constraint fk_log_dni foreign key (dni)
        references Personas(dni),
    entry varchar(250) not null,
    createdAt timestamp default current_timestamp
);


-- datos de ejemplo para la bd --

insert into Personas (dni, nombre, apellido, email, telefono) values
('0', 'Lionel', 'Hutz', 'lionel_hutz@example.com', '123141297'),
('1', 'Marge', 'Simpson', 'MSipmson@example.com', '155514141'),
('2', 'Carlos', 'Gomez', 'carlos.gomez@example.com', '2664112233'),
('3', 'Lucia', 'Fernandez', 'lucia.fernandez@example.com', '2664223344'),
('4', 'Martin', 'Rodriguez', 'martin.rodriguez@example.com', '2664334455'),
('5', 'Sofia', 'Lopez', 'sofia.lopez@example.com', '2664445566'),
('6', 'Gonzalo', 'Diaz', 'gonzalo.diaz@example.com', '2664556677'),
('7', 'Valeria', 'Alvarez', 'valeria.alvarez@example.com', '2664667788'),
('8', 'Esteban', 'Romero', 'esteban.romero@example.com', '2664778899'),
('9', 'Camila', 'Torres', 'camila.torres@example.com', '2664889900'),
('10', 'Mateo', 'Benitez', 'mateo.benitez@example.com', '2664990011'),
('11', 'Florencia', 'Acosta', 'florencia.acosta@example.com', '2664001122');

insert into Usuarios (dni, password, role, avatar) values 
(0, 'AQAAAAIAAYagAAAAEMBNPa0pAIV1tpEWpllLo3M7Tosycs8TLEnpiymtCrH3fZucUu8DSRemLJ7b+DlWqw==', 'Administrador', ''),
(1, 'AQAAAAIAAYagAAAAEMBNPa0pAIV1tpEWpllLo3M7Tosycs8TLEnpiymtCrH3fZucUu8DSRemLJ7b+DlWqw==', 'Administrador', ''),
(2, 'AQAAAAIAAYagAAAAEMBNPa0pAIV1tpEWpllLo3M7Tosycs8TLEnpiymtCrH3fZucUu8DSRemLJ7b+DlWqw==', 'Empleado', ''),
(3, 'AQAAAAIAAYagAAAAEMBNPa0pAIV1tpEWpllLo3M7Tosycs8TLEnpiymtCrH3fZucUu8DSRemLJ7b+DlWqw==', 'Empleado', ''),
(4, 'AQAAAAIAAYagAAAAEMBNPa0pAIV1tpEWpllLo3M7Tosycs8TLEnpiymtCrH3fZucUu8DSRemLJ7b+DlWqw==', 'Empleado', '');

INSERT INTO TipoInmueble (nombre) VALUES 
('casa'), 
('casita'), 
('rancho'), 
('palacio');

insert into Inmuebles (propietario, tipo, direccion, capacidad, precio, porcentaje_reserva, listado) values 
('1', 1, 'Av. Illia 450', 4, 85000.00, 20.00, 1),
('6', 1, 'Pedernera 1230', 5, 95000.00, 25.00, 1),
('7', 1, 'Lavalle 840', 3, 72000.00, 15.00, 1),
('8', 1, 'Junín 512', 4, 80000.00, 20.00, 0),
('6', 2, 'Belgrano 340 Depto 2B', 2, 55000.00, 15.00, 1),
('7', 2, 'Chacabuco 1120 Depto 1A', 2, 50000.00, 15.00, 1),
('8', 2, 'San Martín 920 Monoambiente', 1, 42000.00, 10.00, 1),
('1', 2, 'Rivadavia 670 Depto 4C', 2, 58000.00, 20.00, 0),
('2', 3, 'Ruta 20 Km 9', 6, 130000.00, 30.00, 1),
('7', 3, 'Camino al Dique Lote 14', 8, 160000.00, 30.00, 1),
('8', 3, 'Ruta 147 Km 12', 5, 110000.00, 25.00, 0),
('6', 4, 'B° Las Tipas Mza 4 Lote 2', 8, 280000.00, 35.00, 1),
('2', 4, 'Country Los Quebrachos Casa 12', 10, 350000.00, 40.00, 1),
('1', 4, 'Av. Los Plátanos 2200', 7, 240000.00, 30.00, 1);


insert into ConceptoPago (nombre) values 
('adelanto'), 
('multa'), 
('total'), 
('primer pago');







