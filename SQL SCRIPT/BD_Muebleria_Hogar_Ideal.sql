create database BD_MUEBLERIA

use BD_MUEBLERIA

create table ROL(
IdRol int primary key identity,
Descripcion varchar(50),
FechaRegistro datetime default getdate()
)

go

create table PERMISO(
IdPermiso int primary key identity,
IdRol int references ROL(IdRol),
NombreMenu varchar(100),
FechaRegistro datetime default getdate()
)

go


create table CLIENTE(
IdCliente int primary key identity,
Documento varchar(50),
NombreCompleto varchar(50),
ApellidoPaterno varchar(50),
ApellidoMaterno varchar(50),
Direccion varchar(50),
Telefono varchar(50),
Estado bit,
FechaRegistro datetime default getdate()
)

go

create table USUARIO(
IdUsuario int primary key identity,
Documento varchar(50),
NombreCompleto varchar(50),
ApellidoPaterno varchar(50),
ApellidoMaterno varchar(50),
Correo varchar(50),
Clave varchar(50),
IdRol int references ROL(IdRol),
Estado bit,
FechaRegistro datetime default getdate()
)

go

create table CATEGORIA(
IdCategoria int primary key identity,
Descripcion varchar(100),
Estado bit,
FechaRegistro datetime default getdate()
)

go

create table PROVEEDOR(
IdProveedor int primary key identity,
Documento varchar(50),
RazonSocial varchar(50),
Correo varchar(50),
Telefono varchar(50),
IdCategoria INT REFERENCES CATEGORIA(IdCategoria),
Estado bit,
FechaRegistro datetime default getdate()
)

go

create table PRODUCTO(
IdProducto int primary key identity,
Codigo varchar(50),
Nombre varchar(50),
Descripcion varchar(200),
IdCategoria int references CATEGORIA(IdCategoria),
Stock int not null default 0,
PrecioCompra decimal(10,2) default 0,
PrecioVenta decimal(10,2) default 0,
Estado bit,
FechaRegistro datetime default getdate()
)

go

create table COMPRA(
IdCompra int primary key identity,
IdUsuario int references USUARIO(IdUsuario),
IdProveedor int references PROVEEDOR(IdProveedor),
TipoDocumento varchar(50),
NumeroDocumento varchar(50),
MontoTotal decimal(10,2),
FechaRegistro datetime default getdate()
)


go

create table DETALLE_COMPRA(
IdDetalleCompra int primary key identity,
IdCompra int references COMPRA(IdCompra),
IdProducto int references PRODUCTO(IdProducto),
PrecioCompra decimal(10,2) default 0,
PrecioVenta decimal(10,2) default 0,
Cantidad int,
MontoTotal decimal(10,2),
FechaRegistro datetime default getdate()
)

go

create table VENTA(
IdVenta int primary key identity,
IdUsuario int references USUARIO(IdUsuario),
TipoDocumento varchar(50),
NumeroDocumento varchar(50),
DocumentoCliente varchar(50),
NombreCliente varchar(100),
MontoPago decimal(10,2),
MetodoPago varchar(10),
MontoCambio decimal(10,2),
MontoTotal decimal(10,2),
FechaRegistro datetime default getdate()
)


go


create table DETALLE_VENTA(
IdDetalleVenta int primary key identity,
IdVenta int references VENTA(IdVenta),
IdProducto int references PRODUCTO(IdProducto),
PrecioVenta decimal(10,2),
Cantidad int,
SubTotal decimal(10,2),
FechaRegistro datetime default getdate()
)

go



create table NEGOCIO(
IdNegocio int primary key,
Nombre varchar(60),
RUC varchar(60),
Direccion varchar(60),
Logo varbinary(max) NULL
)

go


/*************************** CREACION DE PROCEDIMIENTOS ALMACENADOS ***************************/
/*--------------------------------------------------------------------------------------------*/

CREATE PROC SP_REGISTRARUSUARIO(
    @Documento varchar(50),
    @NombreCompleto varchar(50),    -- Formato esperado: Nombre
    @ApellidoPaterno varchar(50),
    @ApellidoMaterno varchar(50),
    @Correo varchar(100),
    @Clave varchar(100),              -- Sin el parámetro de confirmar clave
    @IdRol int,
    @Estado bit,
    @IdUsuarioResultado int OUTPUT,
    @Mensaje varchar(500) OUTPUT
)
AS
BEGIN
    SET @IdUsuarioResultado = 0
    SET @Mensaje = ''

    -- Validar que la clave tenga una longitud mínima
    IF LEN(@Clave) < 8 OR LEN(@Clave) > 20
    BEGIN
        SET @Mensaje = 'La clave debe tener entre 8 y 20 caracteres.'
        RETURN
    END

    -- Validar que el documento tenga exactamente 8 dígitos, solo números, y no tenga espacios
    IF LEN(@Documento) = 8 AND @Documento NOT LIKE '%[^0-9]%'
    BEGIN
        -- Validar que el nombre, apellido paterno y apellido materno no contengan números ni caracteres especiales
        IF PATINDEX('%[0-9]%', @NombreCompleto) = 0 AND 
           PATINDEX('%[0-9]%', @ApellidoPaterno) = 0 AND 
           PATINDEX('%[0-9]%', @ApellidoMaterno) = 0 AND 
           PATINDEX('%[^A-Za-z ]%', @NombreCompleto) = 0 AND 
           PATINDEX('%[^A-Za-z ]%', @ApellidoPaterno) = 0 AND 
           PATINDEX('%[^A-Za-z ]%', @ApellidoMaterno) = 0
        BEGIN
            -- Validar que el correo sea de un formato correcto
            IF @Correo LIKE '%_@_%.__%' AND 
               CHARINDEX('@', @Correo) > 1 AND 
               CHARINDEX('.', @Correo) > CHARINDEX('@', @Correo) + 1 AND 
               LEN(LEFT(@Correo, CHARINDEX('@', @Correo) - 1)) BETWEEN 5 AND 20 AND 
               (CHARINDEX('@gmail.com', @Correo) > 0 OR CHARINDEX('@outlook.com', @Correo) > 0)
            BEGIN
                -- Verificar si el documento ya existe en la tabla USUARIO
                IF NOT EXISTS (SELECT * FROM USUARIO WHERE Documento = @Documento)
                BEGIN
                    -- Verificar si el correo ya existe en la tabla USUARIO
                    IF NOT EXISTS (SELECT * FROM USUARIO WHERE Correo = @Correo)
                    BEGIN
                        -- Insertar nuevo usuario
                        INSERT INTO USUARIO (Documento, NombreCompleto, ApellidoPaterno, ApellidoMaterno, Correo, Clave, IdRol, Estado) 
                        VALUES (@Documento, @NombreCompleto, @ApellidoPaterno, @ApellidoMaterno, @Correo, @Clave, @IdRol, @Estado)

                        SET @IdUsuarioResultado = SCOPE_IDENTITY()
                    END
                    ELSE
                    BEGIN
                        SET @Mensaje = 'El correo ya está en uso por otro usuario. Por favor, utiliza otro.'
                    END
                END
                ELSE
                BEGIN
                    SET @Mensaje = 'El número de documento ya existe para otro usuario.'
                END
            END
            ELSE
            BEGIN
                SET @Mensaje = 'El correo no es válido. Asegúrate de que tenga entre 5 y 20 caracteres antes del dominio y que sea de un dominio autorizado (@gmail.com o @outlook.com).'
            END
        END
        ELSE
        BEGIN
            SET @Mensaje = 'El nombre, apellido paterno y apellido materno deben tener solo letras, sin números ni caracteres especiales.'
        END
    END
    ELSE
    BEGIN
        SET @Mensaje = 'El documento debe tener exactamente 8 dígitos, solo números, y no debe contener espacios.'
    END
END
GO


CREATE PROC SP_EDITARUSUARIO(
    @IdUsuario int,
    @Documento varchar(50),
    @NombreCompleto varchar(100),
    @ApellidoPaterno varchar(50),
    @ApellidoMaterno varchar(50),
    @Correo varchar(100),
    @Clave varchar(100),
    @IdRol int,
    @Estado bit,
    @Respuesta bit OUTPUT,
    @Mensaje varchar(500) OUTPUT
)
AS
BEGIN
    SET @Respuesta = 0
    SET @Mensaje = ''

    -- Validar que la clave tenga una longitud mínima
    IF LEN(@Clave) < 8 OR LEN(@Clave) > 20
    BEGIN
        SET @Mensaje = 'La clave debe tener entre 8 y 20 caracteres.'
        RETURN
    END

    -- Validar que el documento tenga exactamente 8 dígitos, solo números, y no tenga espacios
    IF LEN(@Documento) = 8 AND @Documento NOT LIKE '%[^0-9]%'
    BEGIN
        -- Validar que el nombre completo y apellidos no contengan números ni caracteres especiales
        IF LEN(@NombreCompleto) BETWEEN 2 AND 100 
            AND PATINDEX('%[0-9]%', @NombreCompleto) = 0 
            AND PATINDEX('%[^A-Za-z ]%', @NombreCompleto) = 0
            AND LEN(@ApellidoPaterno) > 0 AND LEN(@ApellidoMaterno) > 0
            AND PATINDEX('%[0-9]%', @ApellidoPaterno) = 0 
            AND PATINDEX('%[0-9]%', @ApellidoMaterno) = 0 
            AND PATINDEX('%[^A-Za-z ]%', @ApellidoPaterno) = 0 
            AND PATINDEX('%[^A-Za-z ]%', @ApellidoMaterno) = 0
        BEGIN
            -- Validar que el correo sea de un formato correcto
            IF @Correo LIKE '%_@_%.__%' AND 
               CHARINDEX('@', @Correo) > 1 AND 
               CHARINDEX('.', @Correo) > CHARINDEX('@', @Correo) + 1 AND 
               LEN(LEFT(@Correo, CHARINDEX('@', @Correo) - 1)) BETWEEN 5 AND 20 AND 
               (CHARINDEX('@gmail.com', @Correo) > 0 OR CHARINDEX('@outlook.com', @Correo) > 0)
            BEGIN
                -- Verificar si el documento ya existe para otro usuario
                IF NOT EXISTS (SELECT * FROM USUARIO WHERE Documento = @Documento AND IdUsuario != @IdUsuario)
                BEGIN
                    -- Verificar si el correo ya existe para otro usuario
                    IF NOT EXISTS (SELECT * FROM USUARIO WHERE Correo = @Correo AND IdUsuario != @IdUsuario)
                    BEGIN
                        -- Actualizar usuario existente
                        UPDATE USUARIO 
                        SET Documento = @Documento,
                            NombreCompleto = @NombreCompleto,
                            ApellidoPaterno = @ApellidoPaterno,
                            ApellidoMaterno = @ApellidoMaterno,
                            Correo = @Correo,
                            Clave = @Clave,
                            IdRol = @IdRol,
                            Estado = @Estado
                        WHERE IdUsuario = @IdUsuario

                        SET @Respuesta = 1
                    END
                    ELSE
                    BEGIN
                        SET @Mensaje = 'El correo ya está en uso por otro usuario. Por favor, utiliza otro.'
                    END
                END
                ELSE
                BEGIN
                    SET @Mensaje = 'El número de documento ya existe para otro usuario.'
                END
            END
            ELSE
            BEGIN
                SET @Mensaje = 'El correo no es válido. Asegúrate de que tenga entre 5 y 20 caracteres antes del dominio y que sea de un dominio autorizado (@gmail.com o @outlook.com).'
            END
        END
        ELSE
        BEGIN
            SET @Mensaje = 'El nombre y apellidos deben tener solo letras, sin números ni caracteres especiales.'
        END
    END
    ELSE
    BEGIN
        SET @Mensaje = 'El documento debe tener exactamente 8 dígitos, solo números, y no debe contener espacios.'
    END
END
GO



CREATE PROC SP_ELIMINARUSUARIO(
    @IdUsuario int,
    @Clave varchar(100),              -- Parámetro para la clave
    @Respuesta bit OUTPUT,
    @Mensaje varchar(500) OUTPUT
)
AS
BEGIN
    SET @Respuesta = 0
    SET @Mensaje = ''
    DECLARE @pasoreglas bit = 1
    DECLARE @ClaveGuardada varchar(100)

    -- Validar que la clave tenga una longitud mínima
    IF LEN(@Clave) < 8 OR LEN(@Clave) > 20
    BEGIN
        SET @Mensaje = 'La clave debe tener entre 8 y 20 caracteres.'
        RETURN
    END

    -- Verificar la clave del usuario
    SELECT @ClaveGuardada = Clave FROM USUARIO WHERE IdUsuario = @IdUsuario

    IF @ClaveGuardada IS NULL
    BEGIN
        SET @Mensaje = 'El usuario no existe.'
        RETURN
    END

    IF @ClaveGuardada <> @Clave
    BEGIN
        SET @Mensaje = 'La clave ingresada es incorrecta.'
        RETURN
    END

    -- Verificar si el usuario tiene relación con una compra
    IF EXISTS (SELECT * FROM COMPRA C 
               INNER JOIN USUARIO U ON U.IdUsuario = C.IdUsuario
               WHERE U.IDUSUARIO = @IdUsuario)
    BEGIN
        SET @pasoreglas = 0
        SET @Respuesta = 0
        SET @Mensaje = @Mensaje + 'No se puede eliminar porque el usuario está relacionado a una COMPRA.\n' 
    END

    -- Verificar si el usuario tiene relación con una venta
    IF EXISTS (SELECT * FROM VENTA V
               INNER JOIN USUARIO U ON U.IdUsuario = V.IdUsuario
               WHERE U.IDUSUARIO = @IdUsuario)
    BEGIN
        SET @pasoreglas = 0
        SET @Respuesta = 0
        SET @Mensaje = @Mensaje + 'No se puede eliminar porque el usuario está relacionado a una VENTA.\n' 
    END

    -- Si el usuario no tiene relaciones con compras o ventas, proceder a eliminar
    IF @pasoreglas = 1
    BEGIN
        DELETE FROM USUARIO WHERE IdUsuario = @IdUsuario
        SET @Respuesta = 1 
        SET @Mensaje = 'El usuario ha sido eliminado correctamente.'
    END
END
GO




/* ---------- PROCEDIMIENTOS PARA CATEGORIA -----------------*/

CREATE PROC SP_RegistrarCategoria(
    @Descripcion varchar(50),
    @Estado bit,
    @Resultado int OUTPUT,
    @Mensaje varchar(500) OUTPUT
)
AS
BEGIN
    SET @Resultado = 0
    SET @Mensaje = ''

    -- Validar que la descripción tenga una longitud válida y no contenga números ni caracteres especiales
    IF LEN(@Descripcion) < 2 OR LEN(@Descripcion) > 50 OR
       PATINDEX('%[0-9]%', @Descripcion) > 0 OR 
       PATINDEX('%[^A-Za-z ]%', @Descripcion) > 0
    BEGIN
        SET @Mensaje = 'La descripción debe tener entre 2 y 50 caracteres y no puede contener números ni caracteres especiales.'
        RETURN
    END

    -- Verificar si la categoría ya existe
    IF NOT EXISTS (SELECT * FROM CATEGORIA WHERE Descripcion = @Descripcion)
    BEGIN
        INSERT INTO CATEGORIA(Descripcion, Estado) 
        VALUES (@Descripcion, @Estado)
        SET @Resultado = SCOPE_IDENTITY()
    END
    ELSE
    BEGIN
        SET @Mensaje = 'No se puede repetir la descripción de una categoría.'
    END
END
GO



CREATE PROCEDURE sp_EditarCategoria(
    @IdCategoria int,
    @Descripcion varchar(50),
    @Estado bit,
    @Resultado bit OUTPUT,
    @Mensaje varchar(500) OUTPUT
)
AS
BEGIN
    SET @Resultado = 1
    SET @Mensaje = ''

    -- Validar que la descripción tenga una longitud válida y no contenga números ni caracteres especiales
    IF LEN(@Descripcion) < 2 OR LEN(@Descripcion) > 50 OR
       PATINDEX('%[0-9]%', @Descripcion) > 0 OR 
       PATINDEX('%[^A-Za-z ]%', @Descripcion) > 0
    BEGIN
        SET @Mensaje = 'La descripción debe tener entre 2 y 50 caracteres y no puede contener números ni caracteres especiales.'
        SET @Resultado = 0
        RETURN
    END

    -- Verificar si la categoría ya existe
    IF NOT EXISTS (SELECT * FROM CATEGORIA WHERE Descripcion = @Descripcion AND IdCategoria != @IdCategoria)
    BEGIN
        UPDATE CATEGORIA
        SET Descripcion = @Descripcion,
            Estado = @Estado
        WHERE IdCategoria = @IdCategoria
    END
    ELSE
    BEGIN
        SET @Resultado = 0
        SET @Mensaje = 'No se puede repetir la descripción de una categoría.'
    END
END
GO



CREATE PROCEDURE sp_EliminarCategoria(
    @IdCategoria int,
    @Resultado bit OUTPUT,
    @Mensaje varchar(500) OUTPUT
)
AS
BEGIN
    SET @Resultado = 1
    SET @Mensaje = ''

    -- Verificar si la categoría existe antes de eliminar
    IF EXISTS (SELECT * FROM CATEGORIA WHERE IdCategoria = @IdCategoria)
    BEGIN
        -- Verificar si la categoría está relacionada con algún producto
        IF NOT EXISTS (
            SELECT * FROM CATEGORIA c
            INNER JOIN PRODUCTO p ON p.IdCategoria = c.IdCategoria
            WHERE c.IdCategoria = @IdCategoria
        )
        AND NOT EXISTS (
            SELECT * FROM CATEGORIA c
            INNER JOIN PROVEEDOR pr ON pr.IdCategoria = c.IdCategoria
            WHERE c.IdCategoria = @IdCategoria
        )
        BEGIN
            DELETE FROM CATEGORIA WHERE IdCategoria = @IdCategoria
        END
        ELSE
        BEGIN
            SET @Resultado = 0
            SET @Mensaje = 'La categoría no puede ser eliminada porque está relacionada a un producto o proveedor.'
        END
    END
    ELSE
    BEGIN
        SET @Resultado = 0
        SET @Mensaje = 'La categoría no existe.'
    END
END
GO

/* ---------- PROCEDIMIENTOS PARA PRODUCTO -----------------*/

create PROC sp_RegistrarProducto(
@Codigo varchar(20),
@Nombre varchar(30),
@Descripcion varchar(30),
@IdCategoria int,
@Estado bit,
@Resultado int output,
@Mensaje varchar(500) output
)as
begin
	SET @Resultado = 0
	IF NOT EXISTS (SELECT * FROM producto WHERE Codigo = @Codigo)
	begin
		insert into producto(Codigo,Nombre,Descripcion,IdCategoria,Estado) values (@Codigo,@Nombre,@Descripcion,@IdCategoria,@Estado)
		set @Resultado = SCOPE_IDENTITY()
	end
	ELSE
	 SET @Mensaje = 'Ya existe un producto con el mismo codigo' 
	
end

GO

CREATE PROCEDURE sp_ModificarProducto(
    @IdProducto INT,
    @Codigo VARCHAR(20),
    @Nombre VARCHAR(30),
    @Descripcion VARCHAR(30),
    @IdCategoria INT,
    @Estado BIT,
    @Resultado BIT OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
)
AS
BEGIN
    SET @Resultado = 1;
    
    -- Verificar si el código existe para otro producto
    IF NOT EXISTS (SELECT * FROM PRODUCTO WHERE Codigo = @Codigo AND IdProducto != @IdProducto)
    BEGIN
        -- Actualizar el producto
        UPDATE PRODUCTO 
        SET 
            Codigo = @Codigo,
            Nombre = @Nombre,
            Descripcion = @Descripcion,
            IdCategoria = @IdCategoria,
            Estado = @Estado
        WHERE IdProducto = @IdProducto;

        -- Generar el siguiente código
        DECLARE @MaxCodigo INT;
        
        -- Obtener el código máximo actual
        SELECT @MaxCodigo = MAX(CAST(Codigo AS INT)) FROM PRODUCTO;

        -- Aumentar el código y asignarlo al siguiente producto
        SET @Codigo = CAST(@MaxCodigo + 1 AS VARCHAR(20));
    END
    ELSE
    BEGIN
        SET @Resultado = 0;
        SET @Mensaje = 'Ya existe un producto con el mismo código'; 
    END
END
GO



create PROC SP_EliminarProducto(
@IdProducto int,
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
	set @Respuesta = 0
	set @Mensaje = ''
	declare @pasoreglas bit = 1

	IF EXISTS (SELECT * FROM DETALLE_COMPRA dc 
	INNER JOIN PRODUCTO p ON p.IdProducto = dc.IdProducto
	WHERE p.IdProducto = @IdProducto
	)
	BEGIN
		set @pasoreglas = 0
		set @Respuesta = 0
		set @Mensaje = @Mensaje + 'No se puede eliminar porque se encuentra relacionado a una COMPRA\n' 
	END

	IF EXISTS (SELECT * FROM DETALLE_VENTA dv
	INNER JOIN PRODUCTO p ON p.IdProducto = dv.IdProducto
	WHERE p.IdProducto = @IdProducto
	)
	BEGIN
		set @pasoreglas = 0
		set @Respuesta = 0
		set @Mensaje = @Mensaje + 'No se puede eliminar porque se encuentra relacionado a una VENTA\n' 
	END

	if(@pasoreglas = 1)
	begin
		delete from PRODUCTO where IdProducto = @IdProducto
		set @Respuesta = 1 
	end

end
go

/* ---------- PROCEDIMIENTOS PARA CLIENTE -----------------*/

CREATE PROC sp_RegistrarCliente(
    @Documento varchar(50),
    @NombreCompleto varchar(50),
    @ApellidoPaterno varchar(50),
    @ApellidoMaterno varchar(50),
    @Direccion varchar(50),
    @Telefono varchar(50),
    @Estado bit,
    @Resultado int OUTPUT,
    @Mensaje varchar(500) OUTPUT
) AS
BEGIN
    SET @Resultado = 0
    SET @Mensaje = ''

 -- Validar documento (8 dígitos numéricos sin espacios)
    IF LEN(@Documento) = 8 AND ISNUMERIC(@Documento) = 1 AND CHARINDEX(' ', @Documento) = 0
    BEGIN
        -- Validar teléfono (9 dígitos, empieza con 9, solo números, sin espacios)
        IF LEN(@Telefono) = 9 AND LEFT(@Telefono, 1) = '9' AND ISNUMERIC(@Telefono) = 1 AND CHARINDEX(' ', @Telefono) = 0
        BEGIN
            -- Validar nombre y apellidos
            IF LEN(@NombreCompleto) BETWEEN 2 AND 30
               AND PATINDEX('%[0-9]%', @NombreCompleto) = 0
               AND LEN(LTRIM(RTRIM(@NombreCompleto))) - LEN(REPLACE(LTRIM(RTRIM(@NombreCompleto)), ' ', '')) <= 1
               AND PATINDEX('%[^A-Za-z ]%', @NombreCompleto) = 0
               AND LEN(@ApellidoPaterno) BETWEEN 2 AND 30
               AND PATINDEX('%[0-9 ]%', @ApellidoPaterno) = 0
               AND PATINDEX('%[^A-Za-z]%', @ApellidoPaterno) = 0
               AND LEN(@ApellidoMaterno) BETWEEN 2 AND 30
               AND PATINDEX('%[0-9 ]%', @ApellidoMaterno) = 0
               AND PATINDEX('%[^A-Za-z]%', @ApellidoMaterno) = 0
            BEGIN
                -- Verificar duplicados
                IF NOT EXISTS (SELECT 1 FROM CLIENTE WHERE Documento = @Documento)
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM CLIENTE WHERE Telefono = @Telefono)
                    BEGIN
                        INSERT INTO CLIENTE (Documento, NombreCompleto, ApellidoPaterno, ApellidoMaterno, Direccion, Telefono, Estado)
                        VALUES (@Documento, @NombreCompleto, @ApellidoPaterno, @ApellidoMaterno, @Direccion, @Telefono, @Estado)

                        SET @Resultado = SCOPE_IDENTITY()
                    END
                    ELSE
                        SET @Mensaje = 'El número de teléfono ya está en uso por otro cliente.'
                END
                ELSE
                    SET @Mensaje = 'El número de documento ya existe.'
            END
            ELSE
                SET @Mensaje = 'El nombre o apellidos deben tener entre 2 y 30 caracteres, sin números ni símbolos.'
        END
        ELSE
            SET @Mensaje = 'El teléfono debe tener 9 dígitos, empezar con 9 y no contener espacios.'
    END
    ELSE
        SET @Mensaje = 'El documento debe tener exactamente 8 dígitos numéricos sin espacios.'
END
GO

CREATE PROC sp_ModificarCliente(
    @IdCliente int,
    @Documento varchar(50),
    @NombreCompleto varchar(50),
    @ApellidoPaterno varchar(50),
    @ApellidoMaterno varchar(50),
    @Direccion varchar(50),
    @Telefono varchar(50),
    @Estado bit,
    @Resultado bit OUTPUT,
    @Mensaje varchar(500) OUTPUT
) AS
BEGIN
    SET @Resultado = 1
    SET @Mensaje = ''

    -- Validar documento (8 dígitos numéricos sin espacios)
    IF LEN(@Documento) = 8 AND ISNUMERIC(@Documento) = 1 AND CHARINDEX(' ', @Documento) = 0
    BEGIN
        -- Validar teléfono (9 dígitos, empieza con 9, solo números, sin espacios)
        IF LEN(@Telefono) = 9 AND LEFT(@Telefono, 1) = '9' AND ISNUMERIC(@Telefono) = 1 AND CHARINDEX(' ', @Telefono) = 0
        BEGIN
            -- Validar nombre y apellidos
            IF LEN(@NombreCompleto) BETWEEN 2 AND 30
               AND PATINDEX('%[0-9]%', @NombreCompleto) = 0
               AND LEN(LTRIM(RTRIM(@NombreCompleto))) - LEN(REPLACE(LTRIM(RTRIM(@NombreCompleto)), ' ', '')) <= 1
               AND PATINDEX('%[^A-Za-z ]%', @NombreCompleto) = 0
               AND LEN(@ApellidoPaterno) BETWEEN 2 AND 30
               AND PATINDEX('%[0-9 ]%', @ApellidoPaterno) = 0
               AND PATINDEX('%[^A-Za-z]%', @ApellidoPaterno) = 0
               AND LEN(@ApellidoMaterno) BETWEEN 2 AND 30
               AND PATINDEX('%[0-9 ]%', @ApellidoMaterno) = 0
               AND PATINDEX('%[^A-Za-z]%', @ApellidoMaterno) = 0
            BEGIN
                -- Verificar duplicados
                IF NOT EXISTS (SELECT * FROM CLIENTE WHERE Documento = @Documento AND IdCliente != @IdCliente)
                BEGIN
                    IF NOT EXISTS (SELECT * FROM CLIENTE WHERE Telefono = @Telefono AND IdCliente != @IdCliente)
                    BEGIN
                        UPDATE CLIENTE
                        SET Documento = @Documento,
                            NombreCompleto = @NombreCompleto,
                            ApellidoPaterno = @ApellidoPaterno,
                            ApellidoMaterno = @ApellidoMaterno,
                            Direccion = @Direccion,
                            Telefono = @Telefono,
                            Estado = @Estado
                        WHERE IdCliente = @IdCliente
                    END
                    ELSE
                    BEGIN
                        SET @Resultado = 0
                        SET @Mensaje = 'El número de teléfono ya está en uso por otro cliente.'
                    END
                END
                ELSE
                BEGIN
                    SET @Resultado = 0
                    SET @Mensaje = 'El número de documento ya existe.'
                END
            END
            ELSE
            BEGIN
                SET @Resultado = 0
                SET @Mensaje = 'El nombre o apellidos deben tener entre 2 y 30 caracteres y no contener números ni símbolos.'
            END
        END
        ELSE
        BEGIN
            SET @Resultado = 0
            SET @Mensaje = 'El teléfono debe tener 9 dígitos, empezar con 9 y no contener espacios.'
        END
    END
    ELSE
    BEGIN
        SET @Resultado = 0
        SET @Mensaje = 'El documento debe tener exactamente 8 dígitos, solo números y sin espacios.'
    END
END
GO

/* ---------- PROCEDIMIENTOS PARA PROVEEDOR -----------------*/

CREATE PROC sp_RegistrarProveedor (
    @Documento VARCHAR(50),
    @RazonSocial VARCHAR(50),
    @Correo VARCHAR(50),
    @Telefono VARCHAR(50),
    @Estado BIT,
    @IdCategoria INT, -- Nueva columna para IdCategoria
    @Resultado INT OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
) AS
BEGIN
    SET @Resultado = 0;

    -- Validaciones
    IF LEN(@Documento) != 8 OR @Documento NOT LIKE '[0-12]%'
    BEGIN
        SET @Mensaje = 'El documento debe tener exactamente 12 dígitos y ser numérico.';
        RETURN;
    END

    IF LEN(@Telefono) != 9 OR @Telefono NOT LIKE '9[0-9]%'
    BEGIN
        SET @Mensaje = 'El teléfono debe tener exactamente 9 dígitos y comenzar con 9.';
        RETURN;
    END

    IF NOT (@RazonSocial LIKE '%[^0-9]%' AND LEN(@RazonSocial) BETWEEN 2 AND 30 
        AND LTRIM(RTRIM(@RazonSocial)) = @RazonSocial 
        AND @RazonSocial NOT LIKE '%..%' 
        AND @RazonSocial NOT LIKE '.%')
    BEGIN
        SET @Mensaje = 'El nombre debe tener entre 2 y 30 caracteres, no contener números o múltiples espacios.';
        RETURN;
    END

    -- Validación de correo
    IF LEN(@Correo) < 5 OR LEN(@Correo) > 20 OR 
       @Correo NOT LIKE '%[a-zA-Z]%' OR 
       @Correo NOT LIKE '%_@_%._%' OR 
       (@Correo NOT LIKE '%@gmail.com' AND @Correo NOT LIKE '%@outlook.com') OR
       @Correo LIKE '%[^0-9a-zA-Z@._]%' OR 
       @Correo LIKE '%  %' OR 
       @Correo LIKE '%[^a-zA-Z0-9@._]%'
    BEGIN
        SET @Mensaje = 'El correo debe tener entre 5 y 20 caracteres, contener al menos 5 letras, y ser de un dominio autorizado sin espacios ni símbolos inválidos.';
        RETURN;
    END

    -- Validar que no exista el documento, correo o teléfono
    IF EXISTS (SELECT * FROM PROVEEDOR WHERE Documento = @Documento)
    BEGIN
        SET @Mensaje = 'El número de documento ya existe.';
        RETURN;
    END

    IF EXISTS (SELECT * FROM PROVEEDOR WHERE Correo = @Correo)
    BEGIN
        SET @Mensaje = 'El correo ya está registrado.';
        RETURN;
    END

    IF EXISTS (SELECT * FROM PROVEEDOR WHERE Telefono = @Telefono)
    BEGIN
        SET @Mensaje = 'El número de teléfono ya está registrado.';
        RETURN;
    END

    -- Inserción
    INSERT INTO PROVEEDOR (Documento, RazonSocial, Correo, Telefono, Estado, IdCategoria)
    VALUES (@Documento, @RazonSocial, @Correo, @Telefono, @Estado, @IdCategoria);

    SET @Resultado = SCOPE_IDENTITY();
    SET @Mensaje = 'Proveedor registrado con éxito.';
END
GO

CREATE PROC sp_ModificarProveedor (
    @IdProveedor INT,
    @Documento VARCHAR(50),
    @RazonSocial VARCHAR(50),
    @Correo VARCHAR(50),
    @Telefono VARCHAR(50),
    @Estado BIT,
    @IdCategoria INT, -- Nueva columna para IdCategoria
    @Resultado BIT OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
) AS
BEGIN
    SET @Resultado = 1;

    -- Validaciones
    IF LEN(@Documento) != 8 OR @Documento NOT LIKE '[0-9]%'
    BEGIN
        SET @Mensaje = 'El documento debe tener exactamente 8 dígitos y ser numérico.';
        RETURN;
    END

    IF LEN(@Telefono) != 9 OR @Telefono NOT LIKE '9[0-9]%'
    BEGIN
        SET @Mensaje = 'El teléfono debe tener exactamente 9 dígitos y comenzar con 9.';
        RETURN;
    END

    IF NOT (@RazonSocial LIKE '%[^0-9]%' AND LEN(@RazonSocial) BETWEEN 2 AND 30 
        AND LTRIM(RTRIM(@RazonSocial)) = @RazonSocial 
        AND @RazonSocial NOT LIKE '%..%' 
        AND @RazonSocial NOT LIKE '.%')
    BEGIN
        SET @Mensaje = 'El nombre debe tener entre 2 y 30 caracteres, no contener números o múltiples espacios.';
        RETURN;
    END

    -- Validación de correo
    IF LEN(@Correo) < 5 OR LEN(@Correo) > 20 OR 
       @Correo NOT LIKE '%[a-zA-Z]%' OR 
       @Correo NOT LIKE '%_@_%._%' OR 
       (@Correo NOT LIKE '%@gmail.com' AND @Correo NOT LIKE '%@outlook.com') OR
       @Correo LIKE '%[^0-9a-zA-Z@._]%' OR 
       @Correo LIKE '%  %' OR 
       @Correo LIKE '%[^a-zA-Z0-9@._]%'
    BEGIN
        SET @Mensaje = 'El correo debe tener entre 5 y 20 caracteres, contener al menos 5 letras, y ser de un dominio autorizado sin espacios ni símbolos inválidos.';
        RETURN;
    END

    -- Validar que no exista el documento, correo o teléfono
    IF EXISTS (SELECT * FROM PROVEEDOR WHERE Documento = @Documento AND IdProveedor != @IdProveedor)
    BEGIN
        SET @Mensaje = 'El número de documento ya existe.';
        RETURN;
    END

    IF EXISTS (SELECT * FROM PROVEEDOR WHERE Correo = @Correo AND IdProveedor != @IdProveedor)
    BEGIN
        SET @Mensaje = 'El correo ya está registrado.';
        RETURN;
    END

    IF EXISTS (SELECT * FROM PROVEEDOR WHERE Telefono = @Telefono AND IdProveedor != @IdProveedor)
    BEGIN
        SET @Mensaje = 'El número de teléfono ya está registrado.';
        RETURN;
    END

    -- Actualización
    UPDATE PROVEEDOR 
    SET Documento = @Documento,
        RazonSocial = @RazonSocial,
        Correo = @Correo,
        Telefono = @Telefono,
        Estado = @Estado,
        IdCategoria = @IdCategoria -- Actualizar IdCategoria
    WHERE IdProveedor = @IdProveedor;

    SET @Mensaje = 'Proveedor modificado con éxito.';
END
GO



/* PROCESOS PARA REGISTRAR UNA COMPRA */

CREATE TYPE [dbo].[EDetalle_Compra] AS TABLE(
	[IdProducto] int NULL,
	[PrecioCompra] decimal(18,2) NULL,
	[PrecioVenta] decimal(18,2) NULL,
	[Cantidad] int NULL,
	[MontoTotal] decimal(18,2) NULL
)


GO


CREATE PROCEDURE sp_RegistrarCompra(
@IdUsuario int,
@IdProveedor int,
@TipoDocumento varchar(500),
@NumeroDocumento varchar(500),
@MontoTotal decimal(18,2),
@DetalleCompra [EDetalle_Compra] READONLY,
@Resultado bit output,
@Mensaje varchar(500) output
)
as
begin
	
	begin try

		declare @idcompra int = 0
		set @Resultado = 1
		set @Mensaje = ''

		begin transaction registro

		insert into COMPRA(IdUsuario,IdProveedor,TipoDocumento,NumeroDocumento,MontoTotal)
		values(@IdUsuario,@IdProveedor,@TipoDocumento,@NumeroDocumento,@MontoTotal)

		set @idcompra = SCOPE_IDENTITY()

		insert into DETALLE_COMPRA(IdCompra,IdProducto,PrecioCompra,PrecioVenta,Cantidad,MontoTotal)
		select @idcompra,IdProducto,PrecioCompra,PrecioVenta,Cantidad,MontoTotal from @DetalleCompra


		update p set p.Stock = p.Stock + dc.Cantidad, 
		p.PrecioCompra = dc.PrecioCompra,
		p.PrecioVenta = dc.PrecioVenta
		from PRODUCTO p
		inner join @DetalleCompra dc on dc.IdProducto= p.IdProducto

		commit transaction registro


	end try
	begin catch
		set @Resultado = 0
		set @Mensaje = ERROR_MESSAGE()
		rollback transaction registro
	end catch

end


GO


/* PROCESOS PARA REGISTRAR UNA VENTA */

CREATE TYPE [dbo].[EDetalle_Venta] AS TABLE(
	[IdProducto] int NULL,
	[PrecioVenta] decimal(18,2) NULL,
	[Cantidad] int NULL,
	[SubTotal] decimal(18,2) NULL
)


GO



CREATE PROCEDURE usp_RegistrarVenta(
    @IdUsuario INT,
    @TipoDocumento VARCHAR(500),
    @NumeroDocumento VARCHAR(500),
    @DocumentoCliente VARCHAR(500),
    @NombreCliente VARCHAR(500),
    @MontoPago DECIMAL(18,2),
    @MetodoPago VARCHAR(10),
    @MontoCambio DECIMAL(18,2),
    @MontoTotal DECIMAL(18,2),
    @DetalleVenta [EDetalle_Venta] READONLY,                                      
    @Resultado BIT OUTPUT,
    @Mensaje VARCHAR(500) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        DECLARE @idventa INT = 0
        SET @Resultado = 1
        SET @Mensaje = ''

        BEGIN TRANSACTION registro

        INSERT INTO VENTA(IdUsuario, TipoDocumento, NumeroDocumento, DocumentoCliente, NombreCliente, MontoPago, MetodoPago, MontoCambio, MontoTotal) -- Incluir MetodoPago
        VALUES(@IdUsuario, @TipoDocumento, @NumeroDocumento, @DocumentoCliente, @NombreCliente, @MontoPago, @MetodoPago, @MontoCambio, @MontoTotal)

        SET @idventa = SCOPE_IDENTITY()

        INSERT INTO DETALLE_VENTA(IdVenta, IdProducto, PrecioVenta, Cantidad, SubTotal)
        SELECT @idventa, IdProducto, PrecioVenta, Cantidad, SubTotal FROM @DetalleVenta

        COMMIT TRANSACTION registro

    END TRY
    BEGIN CATCH
        SET @Resultado = 0
        SET @Mensaje = ERROR_MESSAGE()
        ROLLBACK TRANSACTION registro
    END CATCH
END

GO



/****************** INSERTAMOS REGISTROS A LAS TABLAS ******************/
/*---------------------------------------------------------------------*/

 insert into rol (Descripcion)
 values('ADMINISTRADOR')

 GO

  insert into rol (Descripcion)
 values('EMPLEADO')

 GO

INSERT INTO USUARIO(Documento, NombreCompleto, ApellidoPaterno, ApellidoMaterno, Correo, Clave, IdRol, Estado)
VALUES 
('10101010', 'Pedro', 'Sánchez', 'Castro', 'psanchez@gmail.com', '12312345', 1, 1);  

GO

INSERT INTO USUARIO(Documento, NombreCompleto, ApellidoPaterno, ApellidoMaterno, Correo, Clave, IdRol, Estado)
VALUES 
('20202020', 'Diego', 'García', 'Villanueva', 'dgarcia97@gmail.com', '45645678', 2, 1);  

GO


  insert into PERMISO(IdRol,NombreMenu) values
  (1,'menuusuarios'),
  (1,'menumantenedor'),
  (1,'menucompras'),
  (1,'menuclientes'),
  (1,'menuproveedores')
  GO

  insert into PERMISO(IdRol,NombreMenu) values
  (2,'menuventas'),
  (2,'menuclientes')

  GO

insert into NEGOCIO(IdNegocio,Nombre,RUC,Direccion,Logo) values
  (1,'Mueblería Hogar Ideal','20123456789','Av. Tingo María 245, Breña, Lima',null)



  -- DATOS CLIENTES
INSERT INTO CLIENTE (Documento, NombreCompleto, ApellidoPaterno, ApellidoMaterno, Direccion, Telefono, Estado, FechaRegistro)
VALUES
('12345678', 'Juan Alejandro', 'Perez', 'Cardenas', 'Av. Los Olivos 123 - Lima', '987654321', 1, GETDATE()),
('23456789', 'María Alejandra', 'García', 'Marquez', 'Jr. Las Flores 456 - Lima', '923456789', 1, GETDATE()),
('34567890', 'Pedro Augusto', 'Ramirez', 'Hurtado', 'Av. Progreso 789 - Callao', '955555555', 1, GETDATE()),
('45678901', 'Ana Sofia', 'Lopez', 'Machado', 'Calle Los Laureles 321 - Surco', '966666666', 1, GETDATE()),
('56789012', 'Carlos Jose', 'Martinez', 'Huerta', 'Av. Central 654 - San Miguel', '933333333', 1, GETDATE()),
('67890123', 'Laura Alejandra', 'Sanchez', 'Vargas', 'Jr. Primavera 987 - Miraflores', '999999999', 1, GETDATE()),
('78901234', 'Diego Alberto', 'Rodriguez', 'Medina', 'Pasaje Las Violetas 111 - Comas', '977777777', 1, GETDATE()),
('89012345', 'Sofía Milagros', 'Hernandez', 'Silva', 'Av. Libertad 222 - Chorrillos', '944444444', 1, GETDATE()),
('90123456', 'Luisa Maria', 'Gomez', 'Gonzales', 'Jr. San Martín 333 - San Borja', '922222222', 1, GETDATE()),
('01234567', 'Javier Mario', 'Vargas', 'Lloza', 'Av. Las Palmeras 444 - Los Olivos', '911111111', 1, GETDATE());


-- CREACIÓN CATEGORÍA 1 --
INSERT INTO CATEGORIA (Descripcion, Estado, FechaRegistro) 
VALUES ('Sala', 1, GETDATE());

-- CREACIÓN CATEGORÍA 2 --
INSERT INTO CATEGORIA (Descripcion, Estado, FechaRegistro) 
VALUES ('Comedor', 1, GETDATE());

-- CREACIÓN CATEGORÍA 3 --
INSERT INTO CATEGORIA (Descripcion, Estado, FechaRegistro) 
VALUES ('Dormitorio', 1, GETDATE());

-- CREACIÓN CATEGORÍA 4 --
INSERT INTO CATEGORIA (Descripcion, Estado, FechaRegistro) 
VALUES ('Oficina', 1, GETDATE());


-- PROVEEDOR 1 - Sala
INSERT INTO PROVEEDOR (Documento, RazonSocial, Correo, Telefono, IdCategoria, Estado, FechaRegistro)
VALUES ('20123456789', 'Muebles Modernos S.A.C.', 'mueblesmodernos@gmail.com', '987654321', 1, 1, GETDATE());

-- PROVEEDOR 2 - Comedor
INSERT INTO PROVEEDOR (Documento, RazonSocial, Correo, Telefono, IdCategoria, Estado, FechaRegistro)
VALUES ('20456789012', 'Diseños Elegantes E.I.R.L.', 'disenoselegantes@outlook.com', '976543210', 2, 1, GETDATE());

-- PROVEEDOR 3 - Dormitorio
INSERT INTO PROVEEDOR (Documento, RazonSocial, Correo, Telefono, IdCategoria, Estado, FechaRegistro)
VALUES ('20567890123', 'Descanso Ideal S.A.C.', 'descansoideal@gmail.com', '965432109', 3, 1, GETDATE());

-- PROVEEDOR 4 - Oficina
INSERT INTO PROVEEDOR (Documento, RazonSocial, Correo, Telefono, IdCategoria, Estado, FechaRegistro)
VALUES ('20678901234', 'Ofimuebles Perú E.I.R.L.', 'ofimueblesperu@outlook.com', '954321098', 4, 1, GETDATE());

ALTER PROCEDURE sp_RegistrarProducto
    @Codigo        VARCHAR(50),
    @Nombre        VARCHAR(100),
    @Descripcion   VARCHAR(500),
    @IdCategoria   INT,
    @PrecioCompra  DECIMAL(18,2),
    @PrecioVenta   DECIMAL(18,2),
    @Stock         INT,
    @Estado        BIT,
    @Resultado     INT OUTPUT,
    @Mensaje       VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO PRODUCTO
            (Codigo, Nombre, Descripcion, IdCategoria,
             PrecioCompra, PrecioVenta, Stock, Estado)
        VALUES
            (@Codigo, @Nombre, @Descripcion, @IdCategoria,
             @PrecioCompra, @PrecioVenta, @Stock, @Estado);

        SET @Resultado = SCOPE_IDENTITY();
        SET @Mensaje = 'Producto registrado correctamente';
    END TRY
    BEGIN CATCH
        SET @Resultado = 0;
        SET @Mensaje = ERROR_MESSAGE();
    END CATCH
END;

ALTER PROCEDURE sp_ModificarProducto
    @IdProducto    INT,
    @Codigo        VARCHAR(50),
    @Nombre        VARCHAR(100),
    @Descripcion   VARCHAR(500),
    @IdCategoria   INT,
    @PrecioCompra  DECIMAL(18,2),
    @PrecioVenta   DECIMAL(18,2),
    @Stock         INT,
    @Estado        BIT,
    @Resultado     INT OUTPUT,
    @Mensaje       VARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE PRODUCTO
        SET Codigo       = @Codigo,
            Nombre       = @Nombre,
            Descripcion  = @Descripcion,
            IdCategoria  = @IdCategoria,
            PrecioCompra = @PrecioCompra,
            PrecioVenta  = @PrecioVenta,
            Stock        = @Stock,
            Estado       = @Estado
        WHERE IdProducto = @IdProducto;

        SET @Resultado = 1;
        SET @Mensaje = 'Producto modificado correctamente';
    END TRY
    BEGIN CATCH
        SET @Resultado = 0;
        SET @Mensaje = ERROR_MESSAGE();
    END CATCH
END;
