using ImageStore.Business.Interfaces;
using ImageStore.Data.Infrastructure;
using ImageStore.Data;
using ImageStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageStore.Data.EdmxModel;

namespace ImageStore.Business
{
    public class CategoryBusiness : ICategoryBusiness
    {
        //db instance
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private readonly CategoryRepo _categories;

        public CategoryBusiness()
        {
            _categories = new CategoryRepo(_unitOfWork);
        }

        public Response AddUpdate(Categories category)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                //check if a category of the same name exists
                int duplicateCount = _categories.GetAll().Where(x => x.Name == category.Name && x.Id != category.Id).Count();
                if(duplicateCount > 0)
                {
                    res.Message = "warning*A category of the same name already exists";
                }
                else
                {
                    if(category.Id != 0)
                    {
                        Categories existingCategory = _categories.SingleOrDefault(x=> x.Id == category.Id);
                        if(existingCategory != null)
                        {
                            existingCategory.Name = category.Name;
                            _categories.Update(existingCategory);

                            res.Message = "success*Category saved successfully";
                            res.Flag = true;
                        }
                        else
                        {
                            res.Message = "error*Not Found!";
                        }
                    }
                    else
                    {
                        Categories newCategory = new Categories();
                        newCategory.Name = category.Name;
                        newCategory.Flag = true;
                        _categories.Insert(newCategory);

                        res.Message = "success*Category saved successfully";
                        res.Flag = true;
                    }   
                }
            }
            catch { }

            return res;
        }       

        public Response Get(int id=0, string search = "")
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                var count = _categories.GetAll().Count();
                res.Object = _categories.GetAll().Where(x => 
                (id == 0 ? true: (x.Id == id)) 
                && (string.IsNullOrEmpty(search) ? true: x.Name.StartsWith(search)));

                res.Flag = true;
            }
            catch { }
            return res;
        }

        public Response Deactivate(string ids)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                string[] splitIds = ids.Split(',');
                foreach (string id in splitIds)
                {
                    //try parse then continue
                    if (!int.TryParse(id, out int actId))
                        continue;

                    //get category and remove
                    Categories category = _categories.SingleOrDefault(x => x.Id == actId);
                    if (category == null)
                        continue;
                    else
                    {
                        category.Flag = false;
                        _categories.Update(category); 
                    }

                }

                res.Message = "success*Categories Updated Successfully!";
                res.Flag = true;
            }
            catch { }
            return res;
        }

        public Response Activate(string ids)
        {
            Response res = new Response();
            res.Message = " * ";
            try
            {
                string[] splitIds = ids.Split(',');
                foreach (string id in splitIds)
                {
                    //try parse then continue
                    if (!int.TryParse(id, out int actId))
                        continue;

                    //get category and remove
                    Categories category = _categories.SingleOrDefault(x => x.Id == actId);
                    if (category == null)
                        continue;
                    else
                    {
                        category.Flag = true;
                        _categories.Update(category);
                    }
                }

                res.Message = "success*Categories Updated Successfully!";
                res.Flag = true;
            }
            catch { }

            return res;
        }
    }
}
