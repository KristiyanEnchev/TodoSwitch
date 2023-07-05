import PropTypes from 'prop-types';
import { SEO } from 'components';
import { Footer } from '../components/index.jsx';
import ThemingProvider from '../theme/ThemeProvider.jsx';

const MainLayout = ({ children }) => (
  <>
    <SEO
      title="TodoSwitch"
      description="To do app project"
      name="TodoSwitch."
      type="article"
      url="https://kristiyan-enchev-website.web.app/"
      twitterDomain="https://kristiyan-enchev-website.web.app/"
      themeColor="#232e58"
      author="Kristiyan Enchev"
      keywords="React, JavaScript, Todo App"
      robots="index, follow"
      locale="en_US"
      siteName="Todo Switch"
    />{' '}
    <div className="flex flex-col min-h-screen">
      <ThemingProvider>
        <main className="flex-grow bg-gray-300 dark:bg-black">{children}</main>
        <Footer />
      </ThemingProvider>
    </div>
  </>
);

MainLayout.propTypes = {
  children: PropTypes.node.isRequired,
};

export default MainLayout;
