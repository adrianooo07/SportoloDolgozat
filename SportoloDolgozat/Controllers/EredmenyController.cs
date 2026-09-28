using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using SportoloDolgozat.Models;
using SportoloDolgozat.Models.DTOs;

namespace SportoloDolgozat.Controllers
{
    public class EredmenyController
    {
        [Route("eredmeny")]
        [ApiController]
        public class EredmenyController : ControllerBase
        {
            public string ConnectionString = "server=localhost;user=root;password=;database=sportolo13b";

            [HttpGet]
            public List<Eredmeny> GetEredmenyek()
            {
                List<Eredmeny> eredmenyek = new List<Eredmeny>();

                var connection = new MySqlConnection(ConnectionString);
                connection.Open();

                string sql = "SELECT * FROM eredmeny";

                var cmd = new MySqlCommand(sql, connection);

                var data = cmd.ExecuteReader();


                while (data.Read())
                {
                    var eredmeny = new Eredmeny
                    {
                        Id = data.GetInt32("Id"),
                        Competition = data.GetString("Competition"),
                        Description = data.IsDBNull(data.GetOrdinal("Description")) ? null : data.GetString("Description"),
                        ResultTime = data.GetDateTime("ResultTime"),
                        UpdateTime = data.GetDateTime("UpdateTime"),
                        SportoloID = data.GetInt32("SportoloID")
                    };
                    eredmenyek.Add(eredmeny);
                }
                connection.Close();

                return eredmenyek;
            }

            [HttpGet("byId")]
            public object GetEredmenyById(int id)
            {
                var connection = new MySqlConnection(ConnectionString);
                connection.Open();

                string sql = @"SELECT * FROM `eredmeny` WHERE `Id` = @id";

                var cmd = new MySqlCommand(sql, connection);

                cmd.Parameters.AddWithValue("@id", id);

                var datareader = cmd.ExecuteReader();

                object? data = null;

                if (datareader.Read() == true)
                {
                    var eredmeny = new Eredmeny
                    {
                        Id = datareader.GetInt32("Id"),
                        Competition = datareader.GetString("Competition"),
                        Description = datareader.IsDBNull(datareader.GetOrdinal("Description")) ? null : datareader.GetString("Description"),
                        ResultTime = datareader.GetDateTime("ResultTime"),
                        UpdateTime = datareader.GetDateTime("UpdateTime"),
                        SportoloID = datareader.GetInt32("SportoloID")
                    };
                    data = new
                    {
                        message = "Sikeres lekerdezes.",
                        result = eredmeny
                    };
                }
                else
                {
                    data = new
                    {
                        message = "Nincs ilyen eredmeny.",
                        result = ""
                    };
                }

                connection.Close();
                return data;
            }

            [HttpPost]
            public object AddNewEredmeny([FromBody] AddNewEredmenyDto addNewEredmenyDto)
            {
                var connection = new MySqlConnection(ConnectionString);
                connection.Open();

                string sql = @"INSERT INTO `eredmeny` (`Competition`, `Description`, `ResultTime`, `UpdateTime`, `SportoloID`) 
                               VALUES
                               (@competition, @description, @resultTime, @updateTime, @sportoloId)";

                var cmd = new MySqlCommand(sql, connection);

                cmd.Parameters.AddWithValue("@competition", addNewEredmenyDto.Competition);
                cmd.Parameters.AddWithValue("@description", addNewEredmenyDto.Description);
                cmd.Parameters.AddWithValue("@resultTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@updateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@sportoloId", addNewEredmenyDto.SportoloId);

                cmd.ExecuteNonQuery();

                connection.Close();


                return new
                {
                    message = "Sikeres felvetel.",
                    result = addNewEredmenyDto
                };
            }

            [HttpPut]
            public object UpdateEredmeny([FromQuery] int id, [FromBody] UpdateEredmenyDto updateEredmenyDto)
            {
                var connection = new MySqlConnection(ConnectionString);
                connection.Open();

                string sql = @"UPDATE `eredmeny` 
                               SET `Competition` = @competition, 
                                   `Description` = @description, 
                                   `UpdateTime` = @updateTime, 
                                   `SportoloID` = @sportoloId 
                               WHERE `Id` = @id";

                var cmd = new MySqlCommand(sql, connection);

                cmd.Parameters.AddWithValue("@competition", updateEredmenyDto.Competition);
                cmd.Parameters.AddWithValue("@description", updateEredmenyDto.Description);
                cmd.Parameters.AddWithValue("@updateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@sportoloId", updateEredmenyDto.SportoloId);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();

                connection.Close();

                return new
                {
                    message = "Sikeres frissites.",
                    result = updateEredmenyDto
                };
            }

            [HttpDelete]
            public object DeleteEredmeny(int id)
            {
                var connection = new MySqlConnection(ConnectionString);
                connection.Open();

                string sql = @"DELETE FROM `eredmeny` WHERE `Id` = @id";

                var cmd = new MySqlCommand(sql, connection);

                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();

                connection.Close();

                return new
                {
                    message = "Sikeres torles.",
                    result = ""
                };
            }

            //specko
            [HttpGet("sportolo")]
            public object GetSportoloById(int id)
            {
                var connection = new MySqlConnection(ConnectionString);
                connection.Open();

                string sql = @"SELECT `Name`, `Email` FROM `sportolo` WHERE `Id` = @id";

                var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@id", id);

                var datareader = cmd.ExecuteReader();

                object? data = null;

                if (datareader.Read() == true)
                {
                    data = new
                    {
                        message = "Sikeres lekerdezes.",
                        result = new
                        {
                            Name = datareader.GetString("Name"),
                            Email = datareader.IsDBNull(datareader.GetOrdinal("Email")) ? null : datareader.GetString("Email")
                        }
                    };
                }
                else
                {
                    data = new
                    {
                        message = "Nincs ilyen sportolo.",
                        result = ""
                    };
                }
                connection.Close();
                return data;
            }

            [HttpGet(sportoloEredmenyek)]
            public object GetSportoloEredmenyekkel(int id)
            {
                var connection = new MySqlConnection(ConnectionString);
                connection.Open();

                string sql = @"SELECT s.`Name` AS SportoloName, e.`Competition`, e.`Description`
                           FROM `sportolo` s
                           INNER JOIN `eredmeny` e ON s.`Id` = e.`SportoloId`
                           WHERE s.`Id` = @id";

                var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@id", id);
                var datareader = cmd.ExecuteReader();

                string sportoloName = null;

                List<object> eredmenyek = new List<object>();

                while (datareader.Read())
                {
                    sportoloName = datareader.GetString("SportoloName");
                    eredmenyek.Add(
                    new
                    {
                        Competition = datareader.GetString("Competition"),
                        Description = datareader.IsDBNull(datareader.GetOrdinal("Description")) ? null : datareader.GetString("Description")
                    });
                }

                connection.Close();

                if (sportoloName == null)
                {
                    return new
                    {
                        message = "Nincs ilyen sportolo, vagy nincs eredmenye.",
                        result = ""
                    };

                }

                return new
                {
                    message = "Sikeres lekerdezes.",
                    result = new
                    {
                        SportoloName = sportoloName,
                        Eredmenyek = eredmenyek
                    }
                };
            }

            [HttpGet("count")]
            public object GetTotal()
            {
                var connection = new MySqlConnection(ConnectionString);
                connection.Open();

                string sql = "SELECT COUNT(*) FROM `eredmeny`";
                var cmd = new MySqlCommand(sql, connection);

                var count = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();

                return new
                {
                    message = "Sikeres lekerdezes.",
                    result = count
                };
            }

            [HttpGet("sportoloCount")]
            public object GetCountBySportolo(int id)
            {
                var connection = new MySqlConnection(ConnectionString);
                connection.Open();

                string sql = "SELECT COUNT(*) FROM `eredmeny` WHERE `SportoloId` = @id";
                var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@id", id);

                var count = Convert.ToInt32(cmd.ExecuteScalar());

                connection.Close();
                return new
                {
                    message = "Sikeres lekerdezes.",
                    result = new
                    {
                        SportoloId = id,
                        ResultCount = count
                    }
                };
            }
        }
    }
}
