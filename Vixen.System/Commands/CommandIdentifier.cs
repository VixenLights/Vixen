using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Commands {
    public class CommandIdentifier : IEquatable<CommandIdentifier> {
        public readonly byte Platform;
        public readonly byte Category;
        public readonly byte CommandIndex;

        public CommandIdentifier(byte platform, byte category, byte commandIndex) {
            Platform = platform;
            Category = category;
            CommandIndex = commandIndex;
        }

        static public string FormatCommandIdentifierString(byte platform, byte category, byte commandIndex) {
            return _ToString(platform, category, commandIndex);
        }

        public override int GetHashCode() {
            return _ToInt();
        }

        public override bool Equals(object obj) {
            return Equals(obj as CommandIdentifier);
        }

        public virtual bool Equals(CommandIdentifier other) {
            if(object.ReferenceEquals(other, this)) {
                return true;
            } else if(other == null) {
                return false;
            } else if(other.GetHashCode() != this.GetHashCode()) {
                return false;
                // any number of property tests go here...
            } else {
                return true;
            }
        }
        public static bool operator ==(CommandIdentifier left, CommandIdentifier right) {
            if(((object)left) == null) {
                return ((object)right) == null;
            } else {
                return left.Equals(right);
            }
        }

        static public bool operator !=(CommandIdentifier left, CommandIdentifier right) {
            return !(left == right);
        }

        public static implicit operator int(CommandIdentifier value) {
            return value._ToInt();
        }

        public static implicit operator CommandIdentifier(int value) {
            return new CommandIdentifier(
                (byte)((value >> 16) & 0xff),
                (byte)((value >> 8) & 0xff),
                (byte)(value & 0xff)
                );
        }

        public static implicit operator string(CommandIdentifier value) {
            return (value != null) ? _ToString(value.Platform, value.Category, value.CommandIndex) : null;
        }

        public static implicit operator CommandIdentifier(string value) {
            if(value.Length != 6) return null;
            try {
                byte platform, category, index;
                platform = byte.Parse(value.Substring(0, 2));
                category = byte.Parse(value.Substring(2, 2));
                index = byte.Parse(value.Substring(4, 2));
                return new CommandIdentifier(platform, category, index);
            } catch {
                return null;
            }
        }

        private int _ToInt() {
            return ((Platform << 16) | (Category << 8) | CommandIndex);
        }

        static private string _ToString(byte platform, byte category, byte commandIndex) {
            return string.Format("{0:X2}{1:X2}{2:X2}", platform, category, commandIndex);
        }

        public override string ToString() {
            return _ToString(Platform, Category, CommandIndex);
        }
    }
}
