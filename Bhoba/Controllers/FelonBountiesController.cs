﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bhoba.Data;
using Bhoba.Models;
using Bhoba.Models.FelonBountyViewModel;

namespace Bhoba.Controllers
{
    public class FelonBountiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FelonBountiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FelonBounties
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FelonBounties.Include(f => f.BailBondsman).Include(f => f.Felon);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FelonBounties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var felonBounty = await _context.FelonBounties
                .Include(f => f.BailBondsman)
                .Include(f => f.Felon)
                .FirstOrDefaultAsync(m => m.FelonBountyId == id);
            if (felonBounty == null)
            {
                return NotFound();
            }

            return View(felonBounty);
        }

        // GET: FelonBounties/Create
        public IActionResult Create(int? id)
        {
            List<SelectListItem> bailbondsmans = _context.BailBondsmans.Select(bb => new SelectListItem(bb.Name, bb.BailBondsmanId.ToString())).ToList();

            FelonBountyCreateViewModel createViewModel = new FelonBountyCreateViewModel();

            bailbondsmans.Insert(0, new SelectListItem
            {
                Text = "Choose a Bail Bond Agency",
                Value = "0"
            });
            createViewModel.BailBondsmans = bailbondsmans;
            return View(createViewModel);
        }

        // POST: FelonBounties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, FelonBountyCreateViewModel fb)
        {
            fb.FelonBounty.BondClosed = false;
            fb.FelonBounty.FelonId = id;
            fb.FelonBounty.BailBondsmanId = fb.BailBondsmansId;
            if (fb.BailBondsmansId == 0)
            {
                List<SelectListItem> bailbondsmans = _context.BailBondsmans.Select(bb => new SelectListItem(bb.Name, bb.BailBondsmanId.ToString())).ToList();

                FelonBountyCreateViewModel createViewModel = new FelonBountyCreateViewModel();

                bailbondsmans.Insert(0, new SelectListItem
                {
                    Text = "Choose a Bail Bond Agency",
                    Value = "0"
                });
                createViewModel.BailBondsmans = bailbondsmans;
                if (fb.ErrorMsg != null)
                {
                    createViewModel.ErrorMsg = "Please select a Bail Bondsman";
                    return View(createViewModel);
                }
                return View(createViewModel);
            }
            if (ModelState.IsValid)
            {
                _context.Add(fb.FelonBounty);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Felons", new { id });
            }
            return View(fb);
        }

        // GET: FelonBounties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            FelonBountyEditViewModel createview = new FelonBountyEditViewModel();
            if (id == null)
            {
                return NotFound();
            }

            createview.FelonBounty = await _context.FelonBounties
                                            .Where(fb => fb.FelonBountyId == id)
                                            .Include(fb => fb.BailBondsman)
                                            .Include(fb => fb.Felon)
                                            .FirstOrDefaultAsync();

            if (createview.FelonBounty == null)
            {
                return NotFound();
            }

            return View(createview);
        }

        // POST: FelonBounties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FelonBountyEditViewModel returnView)
        {
            if (id != returnView.FelonBounty.FelonBountyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(returnView.FelonBounty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FelonBountyExists(returnView.FelonBounty.FelonBountyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Felons", new { id = returnView.FelonBounty.FelonId});
            }
            return View(returnView);
        }

        // GET: FelonBounties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var felonBounty = await _context.FelonBounties
                .Include(f => f.BailBondsman)
                .Include(f => f.Felon)
                .FirstOrDefaultAsync(m => m.FelonBountyId == id);
            if (felonBounty == null)
            {
                return NotFound();
            }

            return View(felonBounty);
        }

        // POST: FelonBounties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var felonBounty = await _context.FelonBounties.FindAsync(id);
            _context.FelonBounties.Remove(felonBounty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FelonBountyExists(int id)
        {
            return _context.FelonBounties.Any(e => e.FelonBountyId == id);
        }
    }
}
