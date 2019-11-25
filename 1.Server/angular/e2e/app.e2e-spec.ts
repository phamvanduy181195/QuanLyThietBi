import { NewCMTemplatePage } from './app.po';

describe('NewCM App', function() {
  let page: NewCMTemplatePage;

  beforeEach(() => {
    page = new NewCMTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
