import { useState, memo } from 'react';
import PropTypes from 'prop-types';
import PasswordStrengthBar from 'react-password-strength-bar';

const FormField = memo(
  ({ label, type, value, onChange, onFocus, error, id, ...props }) => {
    const [showPassword, setShowPassword] = useState(false);
    const isPasswordField = type === 'password' || type === 'confirmPassword';
    const inputType = isPasswordField
      ? showPassword
        ? 'text'
        : 'password'
      : type;
    const isPassword = type === 'password';

    const togglePasswordVisibility = () => setShowPassword((prev) => !prev);

    return (
      <div className="mb-2">
        <div className="relative">
          <label htmlFor={id} className="text-gray-400 block mb-1">
            {label}
          </label>
          <input
            id={id}
            type={inputType}
            value={value}
            onChange={onChange}
            onFocus={onFocus}
            className="text-white rounded-lg bg-gray-700 w-full p-2 focus:bg-gray-500 focus:outline-none"
            {...props}
          />
          {isPasswordField && (
            <button
              onClick={togglePasswordVisibility}
              type="button"
              className="absolute right-3 top-10 text-xs text-gray-400"
            >
              {showPassword ? 'Hide' : 'Show'}
            </button>
          )}
          {isPassword && <PasswordStrengthBar password={value} />}
          {error && <span className="text-redExtra text-sm">{error}</span>}
        </div>
      </div>
    );
  }
);

FormField.displayName = 'FormField';

FormField.propTypes = {
  label: PropTypes.string.isRequired,
  type: PropTypes.string.isRequired,
  value: PropTypes.string.isRequired,
  onChange: PropTypes.func.isRequired,
  onFocus: PropTypes.func,
  error: PropTypes.string,
  id: PropTypes.string.isRequired,
};

export default FormField;
