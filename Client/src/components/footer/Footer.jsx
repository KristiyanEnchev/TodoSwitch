import { memo } from 'react';
import { useSelector } from 'react-redux';

const Footer = () => {
  const { currentColor } = useSelector((state) => state.theme);
  const social = [
    {
      id: 1,
      url: 'https://www.facebook.com',
      icon: <i className="fab fa-facebook-f"></i>,
    },
    {
      id: 2,
      url: 'https://www.twitter.com',
      icon: <i className="fab fa-twitter"></i>,
    },
    {
      id: 3,
      url: 'https://www.linkedin.com',
      icon: <i className="fab fa-linkedin"></i>,
    },
  ];

  return (
    <footer className="bg-gray-900 text-white border-t border-gray-800">
      <div className="container mx-auto text-center p-4">
        <div className="flex justify-center space-x-5 mb-4">
          <ul className="flex space-x-5">
            {social.map((socialIcon) => {
              const { id, url, icon } = socialIcon;
              return (
                <li key={id}>
                  <a target="_blank" rel="noreferrer" href={url}>
                    {icon}
                  </a>
                </li>
              );
            })}
          </ul>
        </div>
        <span style={{ color: currentColor }}>
          © {new Date().getFullYear()} Copyright:
        </span>
        <a
          style={{ color: currentColor }}
          className="ml-2"
          target="_blank"
          rel="noreferrer"
          href="https://kristiyan-enchev-website.web.app/"
        >
          Kristiyan Enchev
        </a>
      </div>
    </footer>
  );
};

export default memo(Footer);
