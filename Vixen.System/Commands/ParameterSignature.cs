using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Vixen.Commands {
    public class ParameterSignature : IEnumerable<ParameterSpecification> {
        private ParameterSpecification[] _parameters = null;

		private const string PARAMETER_DELIMITER = ",";

		public ParameterSignature(params ParameterSpecification[] parameters) {
			_parameters = parameters;
		}

        public ParameterSignature(string commandParameters) {
            // commandParameters is in the form "type name,type name, ..."
            List<ParameterSpecification> parameters = new List<ParameterSpecification>();
			string trimmedParameterString;
            foreach(string parameterString in commandParameters.Split(new[] { PARAMETER_DELIMITER }, StringSplitOptions.RemoveEmptyEntries)) {
				trimmedParameterString = parameterString.Trim();
                if(!string.IsNullOrEmpty(trimmedParameterString)) {
					parameters.Add(new ParameterSpecification(trimmedParameterString));
                }
            }
            _parameters = parameters.ToArray();
        }

        public ParameterSpecification this[int index] {
            get { return _parameters[index]; }
            set { _parameters[index] = value; }
        }

        public int Count {
            get { return _parameters.Length; }
        }

        public override string ToString() {
			return string.Join(PARAMETER_DELIMITER, (object[])_parameters);
        }

		public IEnumerator<ParameterSpecification> GetEnumerator() {
            foreach(ParameterSpecification parameter in _parameters) {
                yield return parameter;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _parameters.GetEnumerator();
        }
    }

}
