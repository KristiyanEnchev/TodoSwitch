import { DNA } from 'react-loader-spinner';
import PropTypes from 'prop-types';
import { memo } from 'react';

const Spinner = ({ bg, styling }) => {
  let backgroundColor = bg !== undefined ? bg : 'bg-gray-800';
  let style =
    styling !== undefined ? '' : 'grid grid-cols-1 h-screen h-screen1 w-full';

  return (
    <div className={`${style}`}>
      <div className={`${backgroundColor} flex flex-col justify-center`}>
        <DNA
          visible={true}
          height="350"
          width="350"
          ariaLabel="dna-loading"
          wrapperStyle={{ alignSelf: 'center' }}
          wrapperClass="dna-wrapper"
        />
      </div>
    </div>
  );
};

Spinner.propTypes = {
  bg: PropTypes.string,
  styling: PropTypes.string,
};

Spinner.displayName = 'Spinner';

export default memo(Spinner);
