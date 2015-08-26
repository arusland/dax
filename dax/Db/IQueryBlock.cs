/*
 * Copyright 2015 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Data;
using System.Threading.Tasks;

namespace dax.Db
{
    public interface IQueryBlock
    {
        DataTable Table
        {
            get;
        }

        int PageSize
        {
            get;
        }

        int PageIndex
        {
            get;
            set;
        }

        bool IsEmpty
        {
            get;
        }

        bool IsSkipped
        {
            get;
        }

        string QueryText
        {
            get;
        }

        /// <summary>
        /// Executed time in milliseconds
        /// </summary>
        long ElapsedTime
        {
            get;
        }

        void Update();

        void NextPage();

        void PrevPage();

        Task UpdateAsync();

        Task NextPageAsync();

        Task PrevPageAsync();
    }
}
