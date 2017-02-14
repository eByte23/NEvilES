using NEvilES;
using System;

namespace NEvilES.SampleMvc.Commands
{
    public class NewNote : ICommand
	{
		public Guid StreamId { get; set; }
        public string Comment { get; set; }
    }
}