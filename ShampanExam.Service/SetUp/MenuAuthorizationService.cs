﻿using ShampanExam.Repository.Common;
using ShampanExam.Repository.SetUp;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.SetUpVMs;
using ShampanExam.ViewModel.Utility;
using System.Data.SqlClient;

namespace ShampanExam.Service.SetUp
{
    public class MenuAuthorizationService
    {
        CommonRepository _commonRepo = new CommonRepository();

        public async Task<ResultVM> Insert(UserRoleVM urm)
        {
          
            CommonRepository _commonRepo = new CommonRepository();
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();

            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();
                #region Check Exist Data
                string[] conditionField = { "Name" };
                string[] conditionValue = { urm.Name.Trim()};

                bool exist = _commonRepo.CheckExists("Role", conditionField, conditionValue, conn, transaction);

                if (exist)
                {
                    result.Message = "Data Already Exist!";
                    throw new Exception("Data Already Exist!");
                }
                #endregion
                // string code = _commonRepo.CodeGenerationNo(CodeGroup, CodeName, conn, transaction);
                result = await _repo.Insert(urm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
                
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> Update(UserRoleVM urm)
        {
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();
                #region Check Exist Data
                string[] conditionField = { "Id not", "Name" };
                string[] conditionValue = { urm.Id.ToString(), urm.Name.Trim()};

                bool exist = _commonRepo.CheckExists("Role", conditionField, conditionValue, conn, transaction);

                if (exist)
                {
                    result.Message = "Data Already Exist!";
                    throw new Exception("Data Already Exist!");
                }
                #endregion
                result = await _repo.Update(urm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> GetRoleIndexData(GridOptions options)
        {
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.GetRoleIndexData(options, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.ToString();
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> RoleMenuInsert(RoleMenuVM urm)
        {

            CommonRepository _commonRepo = new CommonRepository();
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();

            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();
                //#region Check Exist Data
                //string[] conditionField = { "Name" };
                //string[] conditionValue = { urm.Name.Trim() };

                //bool exist = _commonRepo.CheckExists("Role", conditionField, conditionValue, conn, transaction);

                //if (exist)
                //{
                //    result.Message = "Data Already Exist!";
                //    throw new Exception("Data Already Exist!");
                //}
                //#endregion


                result = await _repo.RoleMenuDelete(urm);

                RoleMenuVM master = new RoleMenuVM();
                bool isSave = false;
                foreach (var item in urm.roleMenuList)
                {
                    if (item.MenuId > 0 && item.IsChecked)
                    {
                        isSave = true;
                        item.CreatedBy = urm.CreatedBy;
                        item.CreatedOn = urm.CreatedOn;
                        item.CreatedFrom = urm.CreatedFrom;
                        item.RoleId = urm.RoleId;
                        result = await _repo.RoleMenuInsert(item);
                    }
                }

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;

            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }
        


    }


}
