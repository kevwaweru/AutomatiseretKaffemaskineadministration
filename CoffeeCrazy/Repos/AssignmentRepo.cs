﻿using CoffeeCrazy.Interfaces;
using CoffeeCrazy.Models;
using Microsoft.Data.SqlClient;

namespace CoffeeCrazy.Repos
{
    public class AssignmentRepo : IAssignmentRepo
    {
        private readonly string _connectionString;
       

        public AssignmentRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'Kaffe Maskine Database' not found.");
        }


       /// <summary>
       /// 
       /// </summary>
       /// <param name="assignment"></param>
       /// <returns></returns>
        public async Task CreateAsync(Assignment assignment)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string SQLquery = @"
                                    INSERT INTO Assignments (Title, Comment, CreateDate, IsCompleted)
                                    VALUES (@Title, @Comment, @CreateDate, @IsCompleted)";

                    using var command = new SqlCommand(SQLquery, connection);
                    command.Parameters.AddWithValue("@Title", assignment.Title);
                    command.Parameters.AddWithValue("@Comment", assignment.Comment);
                    command.Parameters.AddWithValue("@CreateDate", assignment.CreateDate);
                    command.Parameters.AddWithValue("@IsCompleted", assignment.IsCompleted);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (SqlException ex)
            {
                // SQL Errors
                Console.Error.WriteLine($"SQL error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Other Errors
                Console.Error.WriteLine($"Mistakes has happened: {ex.Message}");
                throw;
            }
        }

        
        public async Task DeleteAsync(Assignment toBeDeletedAssignment)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string sqlQuery = "DELETE FROM Assignments WHERE AssignmentId = @AssignmentId";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);

                    command.Parameters.AddWithValue("@AssignmentId", toBeDeletedAssignment.AssignmentId);
                    
                    await connection.OpenAsync();
                    
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("Error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Den metode opdatere en assingment
        /// </summary>
        /// <param name="assignmentToBeUpdated">Angiv hvilke opgave der skal opdateres</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"> den kaster excetion hvis du er dårlig til at kalde den.</exception>
        public async Task UpdateAsync(Assignment assignmentToBeUpdated)
        {
            try
            {
                if (assignmentToBeUpdated == null)
                {
                    throw new ArgumentNullException(nameof(assignmentToBeUpdated), "Du bliver nødt til at sende ny data med, hvis du vil have opdateret opgaven.");
                }

          
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                      Update Assignments
                      Set 
                          Titel = @Titel,
                          Comment = @Comment,
                          CreateDate = @CreateDate, 
                          IsCompleted = @IsCompleted,
                      Where
                          AssignmentId = @AssignmentId";
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.Parameters.AddWithValue("@AssignmentId", assignmentToBeUpdated.AssignmentId);
                        command.Parameters.AddWithValue("@Titel", assignmentToBeUpdated.Title);
                        command.Parameters.AddWithValue("@Comment", (object?)assignmentToBeUpdated.Comment);
                        command.Parameters.AddWithValue("@CreateDate", assignmentToBeUpdated.CreateDate);
                        command.Parameters.AddWithValue("@IsCompleted", assignmentToBeUpdated.IsCompleted);

                        connection.Open(); 
                        await command.ExecuteNonQueryAsync(); //

                    }

                }
            }
            catch (SqlException SqlEx)
            {
                Console.WriteLine("Sql-Exception Error." + SqlEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex);
            }
        }
       
        public async Task<List<Assignment>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async  Task<Assignment> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
