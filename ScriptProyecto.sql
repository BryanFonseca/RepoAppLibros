Create database Biblioteca
use Biblioteca

create table Editorial
(
	IDEditorial int Not null,
	Nombre varchar(50) Not null,
	Ubicacion varchar(150) Not null,
	Correo varchar(200)
)
create table Seccion
(
	IDSeccion int primary key identity,
	Nombre varchar(100) Not null,
	Descripcion varchar(200) Not null,
	CantidadLibros int Not null
)
create table Visitante
(
	IDVisitante int primary key identity,
	Nombres varchar(100) Not null,
	Cedula varchar(10) Not null,
	Celular varchar(10) Not null,
	Direccion varchar(150) Not null
)
create table Libro
(
	IDLibro int primary key identity,
	IDEditorial int Constraint FK_Libro_Editorial Foreign key
	References Editorial(IDEditorial),
	IDSeccion int Constraint FK_Libro_Seccion Foreign key
	References Seccion(IDSeccion),
	Nombre varchar(50) Not null,
	ISBN varchar(13) Not null,
	Prestado bit Not null
)
create table Prestamo
(
	IDPrestamo int primary key identity,
	IDLibro int Constraint FK_Prestamos_Libro Foreign key
	References Libro(IDLibro),
	IDVisitante int Constraint FK_Prestamo_Visitante Foreign key
	References Visitante(IDVisitante),
	FechaPrestamo date Not null,
	FechaDevolucion date Not null
)

CREATE LOGIN [Admin]
WITH PASSWORD = '12345'

CREATE LOGIN [Trabajador]
WITH PASSWORD = '12345'

--Procedimientos almacenados, triggers y cursor----------------------------------------------
create Procedure InsertarVisitante
@Nom varchar(100), @Ced varchar(10), @Celu varchar(10), @Dir varchar(150)
as
	begin try
		begin transaction
			declare @id int = (SELECT ISNULL(MAX(IDVisitante), 0) FROM Visitante) + 1
			insert into Visitante values
			(@id, @Nom, @Ced, @Celu, @Dir)
		commit transaction
	end try
	begin catch
		rollback transaction
		select ERROR_MESSAGE()
	end catch

create Procedure InsertarSeccion
@Nom varchar(100), @Desc varchar(200), @CantLibros int
as
	begin try
		begin transaction
			declare @id int = (SELECT ISNULL(MAX(idseccion), 0) FROM seccion) + 1
			insert into Seccion values
			(@id, @Nom, @Desc, @CantLibros)
		commit transaction
	end try
	begin catch
		rollback transaction
		select ERROR_MESSAGE()
	end catch

create Procedure InsertarEditorial
@Nom varchar(50), @Ubic varchar(150), @Correo varchar(200)
as
	begin try
		begin transaction
			declare @id int = (SELECT ISNULL(MAX(ideditorial), 0) FROM editorial) + 1
			insert into Editorial values
			(@id, @Nom, @Ubic, @Correo)
		commit transaction
	end try
	begin catch
		rollback transaction
		select ERROR_MESSAGE()
	end catch

create Procedure InsertarLibro
@Nom varchar(50), @ISBN varchar(13), @NombEditorial varchar(25), @NombSeccion varchar(25)
as
	declare @ReturnVal bit = 0;
	begin try
			declare @id int = (SELECT ISNULL(MAX(idlibro), 0) FROM Libro) + 1
			declare @IDEditorial int = (select IDEditorial from Editorial where Nombre = @NombEditorial)
			declare @IDSeccion int = (select IDSeccion from seccion where Nombre = @NombSeccion)
			insert into Libro values
			(@id, @IDEditorial, @IDSeccion, @Nom, @ISBN, 0)
			set @ReturnVal = 1
			return (@ReturnVal)
	end try
	begin catch		
		set @ReturnVal = 0
		return (@ReturnVal)
		select ERROR_MESSAGE()
	end catch

create procedure DevolverLibro
@IDPrestamo int
as
	begin try
		begin transaction
			update libro set Prestado = 0 where IDLibro = (select IDLibro from Prestamo where IDPrestamo = @IDPrestamo)
			delete from Prestamo where IDPrestamo = @IDPrestamo
		commit transaction
	end try
	begin catch
		rollback transaction
		select ERROR_MESSAGE()
	end catch
	
create procedure InsertarPrestamo
@IDLibro int, @IDVisitante int
as
	begin try
		begin transaction
			declare @id int = (SELECT ISNULL(MAX(IDPrestamo), 0) FROM Prestamo) + 1
			declare @fechaPrestamo datetime = GetDate() --esto regresa datetime que servirá para el pop up
			declare @fechaDevolucion date = dateadd(DAY, 30, @fechaPrestamo)
			insert into Prestamo values
			(@id, @IDLibro, @IDVisitante, @fechaPrestamo, @fechaDevolucion)
			update Libro set Prestado = 1 where IDLibro = @IDLibro
		commit transaction
	end try
	begin catch
		rollback transaction
		select ERROR_MESSAGE()
	end catch

--el rango es inclusivo, tal que: [@inicio, @final]
create procedure EliminarLibrosRangos
@inicio int, @final int
as
	begin try
		begin transaction
			delete from Libro where IDLibro >= @inicio and IDLibro <= @final
		commit transaction
	end try
	begin catch
		rollback transaction
		select ERROR_MESSAGE()
	end catch	

create procedure EliminarLibro
@id int
as
	begin try
		begin transaction
			delete from Libro where IDLibro = @id
		commit transaction
	end try
	begin catch
		rollback transaction
		select ERROR_MESSAGE()
	end catch

--Trigger que aumenta la cantidad de libros por seccion al insertar un nuevo libro
create trigger tr_InsertLibro
on Libro after insert
as
begin
	SET NOCOUNT ON
	declare @seccion int = (select IDSeccion from inserted)
	update Seccion set CantidadLibros = (select CantidadLibros seccion) + 1
		where IDSeccion = @seccion
end
select * from libro
--aplicar trigger agregando opción que permita al administrador eliminar rangos de libros
create trigger tr_DeleteLibro
on Libro instead of delete
as
begin
	SET NOCOUNT ON
	begin try
		begin transaction
			declare Cursor_Eliminar cursor for
				select IDLibro, IDSeccion from deleted
			open Cursor_Eliminar
			declare @idElim int, @idseccion int
			fetch Cursor_Eliminar into @idElim, @idseccion
			while @@FETCH_STATUS = 0
			begin
				delete from Libro where IDLibro = @idElim
				update Seccion set CantidadLibros = (select CantidadLibros seccion) - 1
					where IDSeccion = @idseccion
				fetch Cursor_Eliminar into @idElim, @idseccion
			end
			close Cursor_Eliminar
			deallocate Cursor_Eliminar
		commit transaction
	end try
	begin catch
		rollback transaction
		select ERROR_MESSAGE()
	end catch
	--en este trigger se hace uso de cursores puesto que se puede eliminar más de un libro a la vez en la aplicación, a diferencia de la inserción, donde solo se puede realizar una a la vez
	--también se hace uso del cursor para actualizar el campo CantidadLibros de la categoría correspondiente
end
----------------------------------------------------------------------------------------------
--Vistas--------------------------------------------------------------------------------------
create view vista_libros as
	select IDLibro, l.Nombre, s.Nombre Seccion, e.Nombre [Editorial], isbn, prestado 
		from libro l inner join Editorial E on e.IDEditorial = l.IDEditorial 
			inner join Seccion s on s.IDSeccion = l.IDSeccion

create view vista_prestamo as
	select p.IDPrestamo as ID, l.Nombre as [Libro], v.Nombres  as [Visitante], convert(date, p.FechaPrestamo) 
		as [Fecha Prestamo], p.FechaDevolucion as [Fecha Devolución] from prestamo p 
			inner join Visitante v on v.IDVisitante = p.IDVisitante 
				inner join Libro l on l.IDLibro = p.IDLibro

select * from vista_libros where prestado = 0
select * from Libro where prestado = 1
-----------------------------------------------------------------------------------------------
--Datos de ejemplo
exec InsertarVisitante 'Bryan Fonseca', '0959316472', '0980619804', 'Naranjito'
exec InsertarVisitante 'Valentin Guerrero', '0948572172', '0959938404', 'Quito'
exec InsertarVisitante 'Andres Arias', '0958473372', '0928395504', 'Guayaquil'
exec InsertarVisitante 'Isaac Viejó', '0959776472', '0935679804', 'Quito'
exec InsertarVisitante 'Gerson Carbache', '0949284172', '0950498404', 'Quito'
exec InsertarVisitante 'Elian Teran', '0999938372', '0958374504', 'Guayaquil'

exec InsertarSeccion 'Tecnología', 'Libros correspondientes a Ingeniería, Electrónica e Informática', 0
exec InsertarSeccion 'Ciencias', 'Libros correspondientes a Facultades de Ciencias', 0
exec InsertarSeccion 'Humanidades', 'Libros correspondientes a Humanidades y Ciencias de la Educación', 0

exec InsertarEditorial 'No Starch Press', 'Estados Unidos', 'nostarchpress_contacto@nsp.com'
exec InsertarEditorial 'O Reilly', 'Estados Unidos', 'oreilly_contact@reilly.com'
exec InsertarEditorial 'Mc Graw Hill', 'Mexico', 'grawhill@ghill.com'

exec InsertarLibro 'How Linux Works: What every Super User should know', '1-59327-035-6', 1, 1
exec InsertarLibro 'The Linux Command Line: A complete Introduction', '1-57433-011-4', 1, 1
exec InsertarLibro 'Ingeniería de Software Un enfoque práctico', '1-93873-011-4', 'Mc Graw Hill', 'Tecnología'

exec InsertarPrestamo 1, 1

exec EliminarLibrosRangos 7, 8 --se eliminan los libros con rangos del 7 al 8

--DBCC CHECKIDENT ('[NombreTabla]', RESEED, 0);
--ninguna tabla tiene su ID como identity para tener más flexibilidad en el desarrollo de la aplicación

--en la aplicación se pintará de rojo la celdas cuyos libros están retrasados (no han sido entregados a tiempo), la lógica para eso será implementada en C#
--al tratar de ingresar un libro, si se puede, se regresará uno 
