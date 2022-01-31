﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HotelLandonBlog.Entities;
using HotelLandonBlog.Repository;
using HotelLandonBlog.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelLandonBlog.UI.Controllers
{
    public abstract class GenericController<TRepository, TEntity, IRazorController> : Controller
        where TRepository : IRepository<TEntity>
        where TEntity : EntityBase
        where IRazorController : IRazorController<TEntity>

    {
        protected readonly IRepository<TEntity> repository;
      

        protected readonly ILogger<GenericController<TRepository, TEntity, IRazorController>> logger;
        

        public GenericController(IRepository<TEntity> repository,
            ILogger<GenericController<TRepository, TEntity, IRazorController>> logger)
        {
            this.repository = repository;
            this.logger = logger;
            
        }

        public async Task<ActionResult<IEnumerable<TEntity>>> Index(string search)
        {
            Stopwatch sw = new();
            sw.Start();
            IEnumerable<TEntity> items = await this.repository.GetAllAsync();
            logger.LogInformation("{ms}ms", sw.ElapsedMilliseconds);
            return View(items);
        }

        public async Task<ActionResult<TEntity>> Create(int id) => View(default(TEntity));


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<TEntity>> Create(int id , TEntity t)
        {

            if (id != t.Id) return View("NotFound");

            if (ModelState.IsValid) return await this.repository.CreateAsync(t);
            if (!ModelState.IsValid) return View(t);

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult<TEntity>> Delete(int id)
        {
            if (id == null)
            {
                return View("NotFound");
            }
            TEntity entity = await this.repository.GetAsync(id);
            if (entity is null)
            {
                return View("NotFound");
            }
            return View(entity);
        }

        public Task<ActionResult<TEntity>> Delete()
        {
            throw new System.NotImplementedException();
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<TEntity>> Details(int id)
        {
            TEntity entity = await this.repository.GetAsync(id);
            if (entity == null)
            {
                return View("NotFound");
            }
            return View(entity);
        }

        public async Task<ActionResult<TEntity>> Edit(int id)
        {
            if (id == null)
            {
                return View("NotFound");
            }
            var entity = await this.repository.GetAsync(id);
            if (entity == null)
            {
                return View("NotFound");
            }
            return View(entity);
        }

        public async Task<ActionResult<TEntity>> Edit(int id, TEntity t)
        {
            if (id != t.Id) return View("NotFound");
            

            if (!ModelState.IsValid) return View(t);

            try
            {
                await this.repository.UpdateAsync(id, t);
            }
            catch (DbUpdateConcurrencyException)
            {
                var tExists = await this.repository.GetAsync(id);

                if (tExists == null)
                    return NotFound();

                throw;
            }


            return RedirectToAction(nameof(Index));
        }


    }
}
