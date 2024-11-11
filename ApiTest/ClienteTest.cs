using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pecas2.Controllers;
using Pecas2.Data;
using Pecas2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Pecas2.Tests
{
    [TestClass]
    public class ClienteControllerTests
    {
        private ClienteController _controller;
        private PecasContext _context;

        [TestInitialize]
        public void Setup()
        {
            
            var options = new DbContextOptionsBuilder<PecasContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PecasContext(options);

            _context.Cliente.AddRange(
                new Cliente { Id = 1, Nome = "João", CPF = "123.456.789-00", Telefone = 123456789, Email = "joao@example.com" },
                new Cliente { Id = 2, Nome = "Maria", CPF = "987.654.321-00", Telefone = 987654321, Email = "maria@example.com" }
            );
            _context.SaveChanges();

            _controller = new ClienteController(_context);
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult_WithListOfClientes()
        {
            var result = await _controller.Index();
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as List<Cliente>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await _controller.Details(null);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Tente acessar um ID inexistente
            var result = await _controller.Details(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsViewResult_WithCliente()
        {
            var result = await _controller.Details(1);
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as Cliente;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Id);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
