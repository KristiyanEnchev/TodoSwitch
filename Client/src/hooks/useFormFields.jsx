import { useState, useCallback } from 'react';

function useFormFields(initialValues) {
  const [fields, setFields] = useState(initialValues);

  const createChangeHandler = useCallback(
    (key, clearError) => (e) => {
      const value = e.target.value;
      setFields((prevFields) => ({ ...prevFields, [key]: value }));
      if (clearError) clearError(key);
    },
    []
  );

  const resetFields = useCallback(
    () => setFields(initialValues),
    [initialValues]
  );

  const capitalizeFirstLetter = useCallback((string) => {
    return string.charAt(0).toUpperCase() + string.slice(1);
  }, []);

  return {
    fields,
    createChangeHandler,
    setFields,
    resetFields,
    capitalizeFirstLetter,
  };
}

export default useFormFields;
