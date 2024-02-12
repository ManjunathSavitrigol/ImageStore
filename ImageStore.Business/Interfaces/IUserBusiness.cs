﻿using ImageStore.Data.EdmxModel;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStore.Business.Interfaces
{
    public interface IUserBusiness
    {
        /// <summary>
        /// Adds or Updates User based on Id.
        /// </summary>
        /// <param categoty="User Object">User to insert</param>
        /// <returns>object Result</returns>
        Response CreateUpdate(User_Details User);

        /// <summary>
        /// Gets categories based on filters Id and search if not provided gets all.
        /// </summary>
        /// <param id="primary key">Primary key to find</param>
        /// <param search="Keyword to search">Primary key to find</param>
        /// <returns>Object Result</returns>
        Response Get(int id = 0, string search = "");

        /// <summary>
        /// Deactivates a User i.e sets Flag to false;
        /// </summary>
        /// <param ids="Primary keys of categories">User to update</param>
        /// <returns>object Result</returns>
        Response Deactivate(string ids);

        /// <summary>
        /// Activates a User i.e sets Flag to false;
        /// </summary>
        /// <param ids="Primary keys of categories">User to update</param>
        /// <returns>object Result</returns>
        Response Activate(string ids);

    }
}
