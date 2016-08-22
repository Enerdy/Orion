using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser.Model;
using System.IO;

namespace Orion.Authorization
{
	/// <summary>
	/// Plain text permission used by the <see cref="PlainTextAccountService"/>.
	/// </summary>
	public class PlainTextPermission : IPermission
	{
		private PlainTextPermissionService _service;
		private IniData _iniData;

		/// <inheritdoc/>
		public string Name => _iniData.Sections["Permission"]["Name"];

		/// <inheritdoc/>
		public string Description => _iniData.Sections["Permission"]["Description"];

		/// <inheritdoc/>
		public IPermission Parent => _service.GetPermissionOrDefault(_iniData.Sections["Permission"]["Parent"]);

		public PlainTextPermission(PlainTextPermissionService service)
		{

		}

		public PlainTextPermission(PlainTextPermissionService service, string permissionName) : this(service)
		{

		}

		public PlainTextPermission(PlainTextPermissionService service, Stream stream) : this(service)
		{

		}

		/// <inheritdoc/>
		public bool HasPermission(IUserAccount userAccount, bool inherit = true)
		{
			throw new NotImplementedException();
		}
	}
}
