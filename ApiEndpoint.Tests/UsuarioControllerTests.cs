using System.Collections.Generic;
using ApiEndpoint.Controllers;
using ApiEndpoint.Models.Request;
using ApiEndpoint.Models.Response;
using Core.Contracts;
using AutoMapper;
using DataEntity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ApiEndpoint.Tests
{
    public class UsuarioControllerTests
    {
        private readonly Mock<IGenericCore<UsuarioEntity>> _mockCore = new Mock<IGenericCore<UsuarioEntity>>();

        [Fact]
        public void GetAll()
        {
            var mappingService = new Mock<IMapper>();

            //Arrange
            var usuarioEntity = new List<UsuarioEntity>
            {
                new UsuarioEntity { Id = 1 },
                new UsuarioEntity { Id = 2 },
                new UsuarioEntity { Id = 3 }
            };

            var usuarioResponseModel = new List<UsuarioResponseModel>
            {
                new UsuarioResponseModel { Id = 1 },
                new UsuarioResponseModel { Id = 2 },
                new UsuarioResponseModel { Id = 3 }
            };

            _mockCore.Setup(x => x.Get()).Returns(usuarioEntity);

            mappingService.Setup(m => m.Map<List<UsuarioEntity>, List<UsuarioResponseModel>>(It.IsAny<List<UsuarioEntity>>())).Returns(usuarioResponseModel);

            var controller = new UsuarioController(_mockCore.Object, mappingService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            //Act
            var apiResponse = controller.Get();

            //Assert
            Assert.Equal(apiResponse.Data[0].Id, usuarioEntity[0].Id);
        }

        [Fact]
        public void GetById()
        {
            var mappingService = new Mock<IMapper>();

            //Arrange
            var usuarioEntity = new UsuarioEntity { Id = 1 };

            var usuarioResponseModel = new UsuarioResponseModel { Id = 1 };

            _mockCore.Setup(x => x.Get(1)).Returns(usuarioEntity);

            mappingService.Setup(m => m.Map<UsuarioEntity, UsuarioResponseModel>(It.IsAny<UsuarioEntity>())).Returns(usuarioResponseModel);

            var controller = new UsuarioController(_mockCore.Object, mappingService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            //Act
            var apiResponse = controller.Get(1);

            //Assert
            Assert.Equal(apiResponse.Data.Id, usuarioEntity.Id);
        }

        [Fact]
        public void Insert()
        {
            var mappingService = new Mock<IMapper>();

            //Arrange
            var usuarioEntity = new UsuarioEntity { Id = 1, Nome="teste" };

            var usuarioRequestModel = new UsuarioRequestModel { Id = 1, Nome = "teste" };

            var usuarioResponseModel = new UsuarioResponseModel { Id = 1, Nome = "teste" };
            

            _mockCore.Setup(x => x.Insert(usuarioEntity)).Returns(usuarioEntity);

            mappingService.Setup(m => m.Map<UsuarioEntity, UsuarioResponseModel>(It.IsAny<UsuarioEntity>())).Returns(usuarioResponseModel);

            var controller = new UsuarioController(_mockCore.Object, mappingService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            //Act
            var apiResponse = controller.Insert(usuarioRequestModel);

            //Assert
            Assert.Equal(apiResponse.Data.Id, usuarioEntity.Id);
            Assert.Equal(apiResponse.Data.Nome, usuarioEntity.Nome);
        }
    }
}