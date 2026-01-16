CREATE OR REPLACE FUNCTION public.sp_search_clientes_by_name(
	search_term character varying)
    RETURNS TABLE(id integer, nombre character varying, apellido character varying, razon_social character varying, cuit character varying, fecha_nacimiento date, telefono_celular character varying, email character varying, fecha_creacion timestamp without time zone, fecha_modificacion timestamp without time zone) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    -- Si el término está vacío, devolver todos los clientes
    IF search_term IS NULL OR TRIM(search_term) = '' THEN
        RETURN QUERY
        SELECT 
            c.id,
            c.nombre,
            c.apellido,
            c.razon_social,
            c.cuit,
            c.fecha_nacimiento,
            c.telefono_celular,
            c.email,
            c.fecha_creacion,
            c.fecha_modificacion
        FROM clientes c
        ORDER BY c.apellido, c.nombre;
    ELSE
        -- Buscar por nombre, apellido o razón social (caracteres centrales)
        RETURN QUERY
        SELECT 
            c.id,
            c.nombre,
            c.apellido,
            c.razon_social,
            c.cuit,
            c.fecha_nacimiento,
            c.telefono_celular,
            c.email,
            c.fecha_creacion,
            c.fecha_modificacion
        FROM clientes c
        WHERE 
            LOWER(c.nombre) LIKE '%' || LOWER(TRIM(search_term)) || '%'
            OR LOWER(c.apellido) LIKE '%' || LOWER(TRIM(search_term)) || '%'
            OR LOWER(c.razon_social) LIKE '%' || LOWER(TRIM(search_term)) || '%'
        ORDER BY c.apellido, c.nombre;
    END IF;
END;
$BODY$;

ALTER FUNCTION public.sp_search_clientes_by_name(character varying)
    OWNER TO postgres;
