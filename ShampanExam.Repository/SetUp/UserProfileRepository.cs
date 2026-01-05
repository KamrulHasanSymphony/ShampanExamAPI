using ShampanExam.Repository.Common;
using ShampanExam.ViewModel.AccountVMs;
using ShampanExam.ViewModel.CommonVMs;
using ShampanExam.ViewModel.KendoCommon;
using ShampanExam.ViewModel.QuestionVM;
using ShampanExam.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;

namespace ShampanExam.Repository.SetUp
{
    public class UserProfileRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(UserProfileVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                string query = @"
INSERT INTO [Users]
(
    Name,FullName, TypeId, BanglaName,
    NIDNo, Mobile, Mobile2,  EmailAddress,
   IsArchive, IsActive, CreatedBy, CreatedOn,ImagePath
)
VALUES 
(
    @Name, @FullName, @TypeId, @BanglaName,
    @NIDNo, @Mobile, @Mobile2, @EmailAddress,
    @IsArchive, @IsActive, @CreatedBy, @CreatedOn,@ImagePath
);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Name", vm.UserName);
                    cmd.Parameters.AddWithValue("@FullName", vm.FullName);
                    cmd.Parameters.AddWithValue("@TypeId", vm.TypeId);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NIDNo", vm.NIDNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile", vm.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile2", vm.PhoneNumber2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailAddress", vm.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath",  (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", 0);
                    cmd.Parameters.AddWithValue("@IsActive", 1);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? "ERP");
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);


                    vm.Id = cmd.ExecuteScalar().ToString();
                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id = vm.Id.ToString();
                    result.DataVM = vm;
                }

                if (isNewConnection)
                {
                    transaction.Commit();
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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


        // Update Method

//        public async Task<ResultVM> Update(UserProfileVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
//        {
//            bool isNewConnection = false;
//            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = vm.Id.ToString(), DataVM = vm };

//            try
//            {
//                if (conn == null)
//                {
//                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
//                    conn.Open();
//                    isNewConnection = true;
//                }

//                if (transaction == null)
//                {
//                    transaction = conn.BeginTransaction();
//                }

//                string query = @"
//UPDATE SalesPersons 
//SET 
//    Code = @Code, Name = @Name, BranchId = @BranchId, ParentId = @ParentId, EnumTypeId = @EnumTypeId,
//    BanglaName = @BanglaName, Comments = @Comments, City = @City, FaxNo = @FaxNo, NIDNo = @NIDNo,
//    Mobile = @Mobile, Mobile2 = @Mobile2, Phone = @Phone, Phone2 = @Phone2, EmailAddress = @EmailAddress,
//    EmailAddress2 = @EmailAddress2, Fax = @Fax, Address = @Address, ZipCode = @ZipCode, 
//    IsArchive = @IsArchive, IsActive= @IsActive, LastModifiedBy = @LastModifiedBy, 
//    LastModifiedOn = GETDATE(),ImagePath = @ImagePath
//WHERE Id = @Id";

//                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
//                {
//                    cmd.Parameters.AddWithValue("@Id", vm.Id);
//                    cmd.Parameters.AddWithValue("@Code", vm.Code);
//                    cmd.Parameters.AddWithValue("@Name", vm.Name);
//                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
//                    cmd.Parameters.AddWithValue("@ParentId", vm.ParentId);
//                    cmd.Parameters.AddWithValue("@EnumTypeId", vm.EnumTypeId);
//                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@City", vm.City ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@NIDNo", vm.NIDNo ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@Mobile", vm.Mobile ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@Mobile2", vm.Mobile2 ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@Phone", vm.Phone ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@Phone2", vm.Phone2 ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@EmailAddress", vm.EmailAddress ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@EmailAddress2", vm.EmailAddress2 ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@Fax", vm.Fax ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@Address", vm.Address ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
//                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
//                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(vm.ImagePath) ? (object)DBNull.Value : vm.ImagePath);

//                    int rowsAffected = cmd.ExecuteNonQuery();
//                    if (rowsAffected > 0)
//                    {
//                        result.Status = "Success";
//                        result.Message = "Data updated successfully.";
//                    }
//                    else
//                    {
//                        result.Message = "No rows were updated.";
//                    }
//                }

//                if (isNewConnection)
//                {
//                    transaction.Commit();
//                }

//                return result;
//            }
//            catch (Exception ex)
//            {
//                if (transaction != null && isNewConnection)
//                {
//                    transaction.Rollback();
//                }

//                result.ExMessage = ex.Message;
//                result.Message = ex.Message;
//                return result;
//            }
//            finally
//            {
//                if (isNewConnection && conn != null)
//                {
//                    conn.Close();
//                }
//            }
//        }


        // Delete Method
        public async Task<ResultVM> Delete(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        // List Method
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    throw new Exception("Database connection fail!");
                }


                string query = $@"
SELECT 
 U.Id 
,U.UserName
,U.FullName
,U.Email
,U.PhoneNumber
,U.PasswordHash
,U.NormalizedPassword
,ISNULL(U.IsHeadOffice,0) IsHeadOffice

FROM 
[{DatabaseHelper.AuthDbName()}].[dbo].AspNetUsers AS U
 LEFT OUTER JOIN [{DatabaseHelper.DBName()}].[dbo].Users SP ON ISNULL(U.UserId,0) = ISNULL(SP.Id,0)
WHERE 1 = 1 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND U.UserId = @Id ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new UserProfileVM
                {
                    Id = row["Id"].ToString(),
                    UserName = row["UserName"].ToString(),
                    FullName = row["FullName"].ToString(),
                    IsHeadOffice = Convert.ToBoolean(row["IsHeadOffice"]),
                    Email = row["Email"].ToString(),
                    PhoneNumber = row["PhoneNumber"].ToString(),
                    Password = row["NormalizedPassword"].ToString(),
                    ConfirmPassword = row["NormalizedPassword"].ToString(),
                    CurrentPassword = row["NormalizedPassword"].ToString(),
                    NormalizedPassword = row["NormalizedPassword"].ToString(),
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;

                return result;

            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
        }

        // ListAsDataTable Method
        public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    throw new Exception("Database connection fail!");
                }

                string query = @"
SELECT 
 U.Id 
,U.UserName
,U.FullName
,U.Email
,U.PhoneNumber
,U.PasswordHash

FROM 

[dbo].[AspNetUsers] AS U

WHERE 1 = 1 ";

                DataTable dataTable = new DataTable();

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = dataTable;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
        }

        // Dropdown Method
        public async Task<ResultVM> Dropdown(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    throw new Exception("Database connection fail!");
                }

                string query = @"
                    SELECT  Id,
		            [Name]UserName
                    ,[FullName]
                    ,[Mobile] PhoneNumber
                    ,[EmailAddress]Email
            FROM Users
            WHERE IsActive = 1 AND IsArchive = 0
            ORDER BY Name
                    ";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new UserProfileVM
                {
                    Id = row["Id"].ToString(),
                    UserName = row["UserName"]?.ToString(),
                    FullName = row["FullName"]?.ToString(),
                    Email = row["Email"]?.ToString(),
                    PhoneNumber = row["PhoneNumber"]?.ToString()
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<ResultVM> GetGridData(GridOptions options, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    throw new Exception("Database connection fail!");
                }

                var data = new GridEntity<UserProfileVM>();

                // Define your SQL query string
                string sqlQuery = $@"
            -- Count query
                    SELECT COUNT(DISTINCT U.Id) AS totalcount
                    FROM 
                    [{DatabaseHelper.AuthDbName()}].[dbo].AspNetUsers AS U
                    LEFT OUTER JOIN [{DatabaseHelper.DBName()}].[dbo].Users SP ON ISNULL(U.UserId,0) = ISNULL(SP.Id,0)
                    WHERE 1 = 1
                    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<UserProfileVM>.FilterCondition(options.filter) + ")" : "") + @"

                    -- Data query with pagination and sorting
                    SELECT * 
                    FROM (
                        SELECT 
                         ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "U.UserName DESC ") + $@") AS rowindex,
                         SP.Id 
                        ,U.UserName
                        ,U.FullName
                        ,U.Email
                        ,U.PhoneNumber
                        ,U.PasswordHash
                        ,ISNULL(U.IsHeadOffice,0) IsHeadOffice                        
	                  , R.Name type

                        FROM 
                        [{DatabaseHelper.AuthDbName()}].[dbo].AspNetUsers AS U
                    LEFT OUTER JOIN [{DatabaseHelper.DBName()}].[dbo].Users SP ON ISNULL(U.UserId,0) = ISNULL(SP.Id,0)
                    LEFT OUTER JOIN [{DatabaseHelper.DBName()}].[dbo].[Role] R ON ISNULL(SP.TypeId,0) = ISNULL(R.Id,0)


                        WHERE 1 = 1
                  
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<UserProfileVM>.FilterCondition(options.filter) + ")" : "") + @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<UserProfileVM>.GetAuthGridData_CMD(options, sqlQuery, "U.UserName");

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = data;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
        }

    }


}
