using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pecas2.Data;
using Pecas2.Models;

namespace Pecas2.Controllers
{
    public class ItemPedidoController : Controller
    {
        private readonly PecasContext _context;

        public ItemPedidoController(PecasContext context)
        {
            _context = context;
        }

        // GET: ItemPedido
        public async Task<IActionResult> Index()
        {
            var pecasContext = _context.ItemPedidos.Include(i => i.Pedido).Include(i => i.Produto);
            return View(await pecasContext.ToListAsync());
        }

        // GET: ItemPedido/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemPedido = await _context.ItemPedidos
                .Include(i => i.Pedido)
                .Include(i => i.Produto)
                .FirstOrDefaultAsync(m => m.PedidoId == id);
            if (itemPedido == null)
            {
                return NotFound();
            }

            return View(itemPedido);
        }

        // GET: ItemPedido/Create
        public IActionResult Create()
        {
            var pedidos = _context.Pedido.ToList();
            var produtos = _context.Produto.ToList();

            if (pedidos == null || !pedidos.Any())
            {
                
                ModelState.AddModelError("", "Nenhum pedido disponível.");
            }

            if (produtos == null || !produtos.Any())
            {
                
                ModelState.AddModelError("", "Nenhum produto disponível.");
            }

            
            ViewData["PedidoId"] = new SelectList(_context.Pedido.Include(l => l.Cliente), "Id", "Cliente.Nome");
            ViewData["ProdutoId"] = new SelectList(produtos, "Id", "Nome");

            
            ViewBag.ProdutosComPreco = produtos.Select(p => new { p.Id, p.Nome, p.Preco }).ToList();

            return View();
        }


        // POST: ItemPedido/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int PedidoId, List<int> ProdutoIds, List<int> Quantidades)
        {
            if (ProdutoIds != null && Quantidades != null && ProdutoIds.Count == Quantidades.Count)
            {
                for (int i = 0; i < ProdutoIds.Count; i++)
                {
                    var produto = _context.Produto.Find(ProdutoIds[i]);
                    if (produto != null)
                    {
                        var itemPedido = new ItemPedido
                        {
                            PedidoId = PedidoId,
                            ProdutoId = ProdutoIds[i],
                            Quantidade = Quantidades[i],
                            // Adiciona o cálculo do preço total diretamente no ItemPedido
                            Subtotal = Quantidades[i] * produto.Preco
                        };

                        _context.ItemPedidos.Add(itemPedido);
                    }
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Caso não haja produtos ou quantidades, retorne à página de criação com erro
            return View();
        }

        // GET: ItemPedido/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemPedido = await _context.ItemPedidos.FindAsync(id);
            if (itemPedido == null)
            {
                return NotFound();
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedido, "Id", "Id", itemPedido.PedidoId);
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Marca", itemPedido.ProdutoId);
            return View(itemPedido);
        }

        // POST: ItemPedido/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PedidoId,ProdutoId,Quantidade,Subtotal")] ItemPedido itemPedido)
        {
            if (id != itemPedido.PedidoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemPedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemPedidoExists(itemPedido.PedidoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedido, "Id", "Id", itemPedido.PedidoId);
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Marca", itemPedido.ProdutoId);
            return View(itemPedido);
        }

        // GET: ItemPedido/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemPedido = await _context.ItemPedidos
                .Include(i => i.Pedido)
                .Include(i => i.Produto)
                .FirstOrDefaultAsync(m => m.PedidoId == id);
            if (itemPedido == null)
            {
                return NotFound();
            }

            return View(itemPedido);
        }

        // POST: ItemPedido/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemPedido = await _context.ItemPedidos.FindAsync(id);
            if (itemPedido != null)
            {
                _context.ItemPedidos.Remove(itemPedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemPedidoExists(int id)
        {
            return _context.ItemPedidos.Any(e => e.PedidoId == id);
        }
    }
}
