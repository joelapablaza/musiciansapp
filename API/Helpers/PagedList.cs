using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    // Clase PagedList que hereda de List<T> y es genérica
    public class PagedList<T> : List<T>
    {
        // Constructor que toma los elementos, el número total de elementos, la página actual y el tamaño de la página
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            // Establece la página actual
            CurrentPage = pageNumber;
            // Calcula el número total de páginas y lo establece
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            // Establece el tamaño de la página
            PageSize = pageSize;
            // Establece el número total de elementos
            TotalCount = count;
            // Agrega los elementos a la lista actual
            AddRange(items);
        }

        // Propiedad que devuelve la página actual
        public int CurrentPage { get; set; }
        // Propiedad que devuelve el número total de páginas
        public int TotalPages { get; set; }
        // Propiedad que devuelve el tamaño de la página
        public int PageSize { get; set; }
        // Propiedad que devuelve el número total de elementos
        public int TotalCount { get; set; }

        // Método estático que se utiliza para crear una instancia de PagedList a partir de una fuente IQueryable
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // Cuenta el número total de elementos de la fuente y lo almacena en una variable
            var count = await source.CountAsync();
            // Obtiene los elementos paginados y los almacena en una variable
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            // Devuelve una instancia de PagedList con los elementos paginados y el número total de elementos
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}