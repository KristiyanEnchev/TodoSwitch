import PropTypes from 'prop-types';
import { memo } from 'react';
import { AlarmClock as Clock } from 'lucide-react';

const AlarmClock = ({ deadline, isDone, message }) => {
  const deadlineDate = new Date(deadline);
  const currentDate = new Date();
  const isLate = currentDate > deadlineDate && !isDone;

  let timeDifference = currentDate - deadlineDate;
  const daysLate = Math.floor(timeDifference / (1000 * 60 * 60 * 24));

  const hoursLate = Math.floor(
    (timeDifference % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)
  );
  const minutesLate = Math.floor(
    (timeDifference % (1000 * 60 * 60)) / (1000 * 60)
  );

  return (
    <div className="flex items-center space-x-2">
      <Clock
        className={`text-4xl ${
          isLate ? 'animate-ring text-red-600' : 'text-black dark:text-white'
        }`}
      ></Clock>
      {isLate && message ? (
        <div className="flex flex-col text-red-600 text-sm font-sans italic">
          <span className="font-sans italic font-semibold">{deadline}</span>
          <span className="font-sans italic font-semibold">
            {message} (
            {daysLate > 0
              ? `${daysLate} ${daysLate === 1 ? 'day' : 'days'} late`
              : `${hoursLate} ${
                  hoursLate === 1 ? 'hour' : 'hours'
                }, ${minutesLate} ${
                  minutesLate === 1 ? 'minute' : 'minutes'
                } late`}
            )
          </span>
        </div>
      ) : (
        <span className="text-gray-600 dark:text-gray-400 text-sm font-sans italic">
          {deadlineDate.toLocaleString()}
        </span>
      )}
    </div>
  );
};

AlarmClock.propTypes = {
  deadline: PropTypes.string.isRequired,
  isDone: PropTypes.bool.isRequired,
  message: PropTypes.string,
};

AlarmClock.displayName = 'AlarmClock';

export default memo(AlarmClock);
