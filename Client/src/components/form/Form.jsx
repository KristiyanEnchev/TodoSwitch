import { memo, useCallback } from 'react';
import PropTypes from 'prop-types';
import FormField from './FormField';
import useFormFields from 'hooks/useFormFields';
import useFormValidation from 'hooks/useFormValidation';

const Form = memo(
  ({
    initialValues,
    validationSchema,
    onSubmit,
    submitButtonText,
    fields,
    titleSection,
    extraContent,
    color,
  }) => {
    const {
      fields: formFields,
      createChangeHandler,
      resetFields,
      capitalizeFirstLetter,
    } = useFormFields(initialValues);
    const { validate, clearError, getFieldError /*isValid*/ } =
      useFormValidation(validationSchema, formFields);

    const handleSubmit = useCallback(
      async (e) => {
        e.preventDefault();
        if (validate(formFields)) {
          await onSubmit(formFields);
          resetFields();
        }
      },
      [validate, formFields, onSubmit, resetFields]
    );

    return (
      <form
        onSubmit={handleSubmit}
        className="max-w-[400px] w-full mx-auto rounded-lg bg-gray-800 dark:bg-gray-300 p-8 px-8"
      >
        {titleSection}

        {fields.map(({ name, type, label }) => (
          <FormField
            key={name}
            label={label || capitalizeFirstLetter(name)}
            type={type || name}
            value={formFields[name]}
            onChange={createChangeHandler(name, () => clearError(name))}
            onFocus={() => clearError(name)}
            error={getFieldError(name)}
            id={name}
          />
        ))}
        <button
          style={{ backgroundColor: color }}
          type="submit"
          className="w-full my-5 py-2 shadow-lg text-white font-semibold rounded-lg cursor-pointer"
          //   disabled={!isValid}
        >
          {submitButtonText}
        </button>
        {extraContent}
      </form>
    );
  }
);

Form.displayName = 'Form';

Form.propTypes = {
  initialValues: PropTypes.object.isRequired,
  validationSchema: PropTypes.object,
  onSubmit: PropTypes.func,
  submitButtonText: PropTypes.string,
  fields: PropTypes.array,
  titleSection: PropTypes.node,
  extraContent: PropTypes.node,
  color: PropTypes.string,
};

export default Form;
