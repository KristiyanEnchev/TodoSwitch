/* eslint-disable no-unused-vars */
import { useState, useCallback, useEffect } from 'react';
import { ZodError } from 'zod';

function useFormValidation(schema, initialFields) {
  const [errors, setErrors] = useState({});
  const [isValid, setIsValid] = useState(() => {
    try {
      schema.parse(initialFields);
      return true;
    } catch (error) {
      return false;
    }
  });

  const validate = useCallback(
    (fields) => {
      try {
        schema.parse(fields);
        setErrors({});
        setIsValid(true);
        return true;
      } catch (error) {
        if (error instanceof ZodError) {
          setErrors(error.flatten().fieldErrors);
        }
        setIsValid(false);
        return false;
      }
    },
    [schema]
  );

  const clearError = useCallback((key) => {
    setErrors((prevErrors) => ({ ...prevErrors, [key]: undefined }));
  }, []);

  const getFieldError = useCallback(
    (key) => {
      return Array.isArray(errors[key]) ? errors[key][0] : undefined;
    },
    [errors]
  );

  useEffect(() => {
    const validateFields = () => {
      try {
        schema.parse(initialFields);
        setIsValid(true);
      } catch (error) {
        setIsValid(false);
      }
    };
    validateFields();
  }, [initialFields, schema]);

  return { validate, errors, setErrors, clearError, getFieldError, isValid };
}

export default useFormValidation;
