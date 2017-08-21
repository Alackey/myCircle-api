using System.Collections.Generic;
using myCircle_api.Controllers.ResponseMessages;
using myCircle_api.Model;
using myCircle_api.Repository;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace myCircle_api.Controllers
{
    [Route("v1/[controller]")]
    public class GroupsController : Controller
    {
        private readonly GroupRepository groupRepository;
        public GroupsController()
        {
            groupRepository = new GroupRepository();
        }
        
        // GET v1/groups
        [HttpGet]
        public IEnumerable<Group> Get()
        {
            return groupRepository.GetAll();
        }

        // GET v1/groups/id
        [HttpGet("{id}")]
        public Group Get(string id)
        {
            return groupRepository.GetById(id);
        }

        // POST v1/groups
        [HttpPost]
        public IActionResult Post([FromBody] Group group)
        {
            var newGroup = groupRepository.Add(group);
            return Json(newGroup);
        }

        // PUT v1/groups/5
        [HttpPut("{id}")]
        public Group Put(string id, [FromBody] Group group)
        {
            return groupRepository.Update(id, group);
        }

        // DELETE v1/groups/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            groupRepository.Delete(id);
        }
    }
}