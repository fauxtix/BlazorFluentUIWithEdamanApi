﻿using EdamanFluentApi.Data;
using EdamanFluentApi.Models.Youtube.Entities;
using EdamanFluentApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EdamanFluentApi.Repositories.Implementations
{
    public class CategoriasRepository : Repository<Categoria>, ICategoriasRepository
    {
        protected new readonly YoutubeDbContext _context;
        public CategoriasRepository(YoutubeDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> GetCategoriesWithMediaEntries()
        {
            var query = from c in _context.Categorias
                        .AsNoTracking()
                        where _context.MediaFiles.Any(m => m.IdGenero == c.Id)
                        orderby c.Descricao
                        select c;

            var resultList = await query.ToListAsync();
            return resultList;
        }
    }
}
