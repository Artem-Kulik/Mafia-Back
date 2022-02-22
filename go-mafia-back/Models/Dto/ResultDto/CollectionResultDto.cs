using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace go_mafia_back.Models.Dto.ResultDto
{
    public class CollectionResultDto<T> : ResultDto
    {
        public ICollection<T> Data { get; set; }
    }
}
