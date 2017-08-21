using System;
using System.Collections.Generic;
using myCircle_api.Controllers.ResponseMessages;
using myCircle_api.Model;
using myCircle_api.Repository;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace myCircle_api.Controllers
{
    [Route("v1/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserRepository userRepository;
        public UsersController()
        {
            userRepository = new UserRepository();
        }
        
        // GET v1/users
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return userRepository.GetAll();
        }

        // GET v1/users/username
        [HttpGet("{username}")]
        public User Get(string username)
        {
            return userRepository.GetByUsername(username);
        }

        // POST v1/users
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            try
            {
                userRepository.Add(user);
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    // Duplicate key
                    case 1048:
                        var error = new Error(e.Number, "UsernameAlreadyExists");
                        return NotFound(error);
                }
            }
            return Json(new {
                data = new {
                    username = user.Username
                }
            });
        }

        // PUT v1/users/username
        [HttpPut("{username}")]
        public void Put(string username, [FromBody] User user)
        {
            userRepository.Update(user);
        }

        // DELETE v1/users/username
        [HttpDelete("{username}")]
        public void Delete(string username)
        {
            userRepository.Delete(username);
        }
    }
}