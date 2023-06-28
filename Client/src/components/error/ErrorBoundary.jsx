import PropTypes from 'prop-types';
import { Component } from 'react';

class ErrorBoundary extends Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false, error: null, errorInfo: null };
  }

  static getDerivedStateFromError(error) {
    return { hasError: true, error };
  }

  componentDidCatch(error, errorInfo) {
    console.error('ErrorBoundary caught an error', error, errorInfo);
    this.setState({ error, errorInfo });
  }

  handleReload = () => {
    window.location.reload();
  };

  render() {
    if (this.state.hasError) {
      return (
        <div className="flex items-center justify-center min-h-screen bg-gray-900 p-4">
          <div className="bg-gray-800 shadow-lg rounded-lg p-6 sm:p-8 max-w-sm sm:max-w-md w-full text-center">
            <h1 className="text-3xl font-bold text-myRed mb-4">
              Oops! Something went wrong.
            </h1>
            <p className="text-gray-300 mb-4 sm:mb-6">
              An unexpected error has occurred. We apologize for the
              inconvenience.
            </p>
            <details className="bg-gray-700 rounded p-4 text-left text-sm text-gray-400 whitespace-pre-wrap">
              {this.state.error?.toString()}
              <br />
              {this.state.errorInfo?.componentStack}
            </details>
            <button
              onClick={this.handleReload}
              className="mt-6 bg-myRed text-white py-2 px-4 rounded hover:bg-rose-700 transition duration-300"
            >
              Reload Page
            </button>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}

ErrorBoundary.propTypes = {
  children: PropTypes.node.isRequired,
};

export default ErrorBoundary;
