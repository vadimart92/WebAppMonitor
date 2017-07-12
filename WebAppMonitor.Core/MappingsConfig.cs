using AutoMapper;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.Core
{
    public class MappingProfile: Profile {
	    public MappingProfile() {
		    CreateMap<ISettings, DataImportSettings>();
		    CreateMap<Settings, DataImportSettings>().ReverseMap();
	    }
	}
}
