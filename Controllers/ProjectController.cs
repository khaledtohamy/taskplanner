﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskPlanner.Models;
using Microsoft.AspNetCore.Identity; 
using System.Security.Claims;

namespace TaskPlanner.Controllers
{
    public class ProjectController : Controller
    {

        [HttpGet("project", Name = "Project_List")]
        public IActionResult Projects() 
        {
            ViewData["Message"] = "Projects";


            var currentUserEmail = User.Identity.Name;

            var projectList = ProjectModel.GetProjectList("", currentUserEmail);
			return this.View("~/Views/Project/Projects.cshtml", projectList);
		}

        [HttpPost("project/delete/{projectId}", Name = "Project_Delete")] 
        public JsonResult DeleteProject(int projectId)
        {
            var currentUserEmail = User.Identity.Name;
            bool isOwner = Permission.IsUserOwnerOfProject(currentUserEmail, projectId);

            if(!isOwner)
                return this.Json(new { isSuccess = false,message="Permission Denied" });

            var res = new ProjectViewModel().DeleteProject(projectId);

            if(res!=null && res.IsSuccess)
                return this.Json(new { isSuccess = true, message = "Project deleted successfully" });

            else
                return this.Json(new { isSuccess = false, message = "Unexpected error occurred" }); 
        }

        [HttpPost("project/favourite/", Name = "Project_Favourite")]
        public JsonResult UpdateFavourite(int projectId,bool isFavourite)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = new ProjectViewModel().UpdateFavouriteList(projectId, userId, isFavourite);

            if (res != null && res.IsSuccess)
            {
                if (isFavourite)
                    return this.Json(new { isSuccess = true, message = "Added to favourite" });
                else
                    return this.Json(new { isSuccess = true, message = "Removed from favourite" });
            }

            else
                return this.Json(new { isSuccess = false, message = "Unexpected error occurred" });
        }

		[HttpPost]
		public ActionResult LoadProject(string projectId)
		{
			var currentUserEmail = User.Identity.Name;
			var projectList = ProjectModel.GetProjectList(projectId, currentUserEmail);
			return this.PartialView("~/Views/Project/_projects.cshtml", projectList);
		}
	
        /// <summary>
        /// Newproject() - Return the partial view
        /// </summary>
        /// <returns>partial view</returns>
        public PartialViewResult Newproject()
        {
            return this.PartialView("~/Views/Project/_addProject.cshtml");
        }

        /// <summary>
        /// AddProjectAsync() - Add project details in DB
        /// </summary>
        /// <param name="description"></param>
        /// <param name="projectname"></param>
        /// <returns></returns>
        public JsonResult AddProjectAsync(string description = "", string projectname = "", string projectId = "")
        {
            ProjectListObjects objects = new ProjectListObjects();
            objects.ProjectDescription = description;
            objects.ProjectName = projectname;
            int id = 0;
            int.TryParse(projectId, out id);
            if(id >0)
                objects.ProjectId = id;
            objects.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserEmail = User.Identity.Name;

            var res = new ProjectViewModel().UpdateProjectDetails(objects, currentUserEmail);
            if (res.IsSuccess)
            {
                return this.Json(new
                {
                    status = true,
                    message = (!string.IsNullOrEmpty(projectId))? "Project Updated successfully." : "New project created successfully."
                });
            }
            else
            {
                return this.Json(new
                {
                    status = false,
                    message = "Unexpected error occurred."
                });
            }
        }

        /// <summary>
        /// AddProjectAsync() - Add project details in DB
        /// </summary>
        /// <param name="description"></param>
        /// <param name="projectname"></param>
        /// <returns></returns>
        public JsonResult EditProjectAsync(string projectId = "")
        {
            int id = 0;
            int.TryParse(projectId, out id);
            var result = ProjectModel.GetProjectDetails(id);
            if (result.ProjectListObjects.Count>0)
            {
                return this.Json(new
                {
                    status = true,
                    description = result.ProjectListObjects[0].ProjectDescription,
                    name = result.ProjectListObjects[0].ProjectName
                });
            }
            else
            {
                return this.Json(new
                {
                    status = false,
                    message = "Unexpected error occurred."
                });
            }
        }

		/// <summary>
		/// Shareproject() - Return the partial view
		/// </summary>
		/// <returns>partial view</returns>
		public PartialViewResult ShareProject(int projectId)
		{
            var currentUserEmail = User.Identity.Name.ToLower().Trim();



            var list = ProjectModel.GetProjectSharedList(projectId, currentUserEmail);
			//return this.PartialView("~/Views/Project/_ShareList.cshtml", list);
			return this.PartialView("~/Views/Project/_ShareProject.cshtml", list);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="projectId"></param>
		/// <param name="email"></param>
		/// <returns></returns>
		public JsonResult ShareEmail(int projectId, string email = "")
		{
            var currentUserEmail = User.Identity.Name.ToLower().Trim() ;

            email = email.ToLower().Trim();
            if (email== currentUserEmail)
                return this.Json(new
                {
                    status = true,
                    message = "Owner already has permission for project."
                });

            

            var res = new ProjectViewModel().UpdateProjectPermissionList(projectId, email,true);
			if (res.IsSuccess)
			{
				return this.Json(new
				{
					status = true,
					message = "Permission added successfully."
				});
			}
			else
			{
				return this.Json(new
				{
					status = false,
					message = "Unexpected error occurred."
				});
			}
		}
 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="permissionId"></param>
		/// <returns></returns>
		public JsonResult RemovePermission(int permissionId)
		{

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = new ProjectViewModel().RemoveProjectPermission(permissionId, userId);



            if (res.IsSuccess)
			{
				return this.Json(new
				{
					status = true,
					message = "Project permission is removed."
				});
			}
			else
			{
				return this.Json(new
				{
					status = false,
					message = "Unexpected error occurred."
				});
			}
		}
	}
}