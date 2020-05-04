﻿#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;

#endregion

namespace SymuEngine.Repository.Networks.Databases
{
    /// <summary>
    ///     List of all the Database
    ///     Use by NetworkDatabases
    /// </summary>
    public class DatabaseCollection
    {
        /// <summary>
        ///     Key => DatabaseId
        ///     Values => List of Databases
        /// </summary>
        public List<Database> List { get; } = new List<Database>();

        public void Add(Database database)
        {
            if (!Contains(database))
            {
                List.Add(database);
            }
        }

        public bool Contains(Database database)
        {
            if (database is null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            return Exists(database.Id);
        }

        public Database GetDatabase(ushort databaseId)
        {
            return List.Find(k => k.Id == databaseId);
        }

        public bool Exists(ushort databaseId)
        {
            return List.Exists(k => k.Id == databaseId);
        }

        public void Clear()
        {
            List.Clear();
        }
    }
}