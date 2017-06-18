import { WebAppMonitorPage } from './app.po';

describe('web-app-monitor App', () => {
  let page: WebAppMonitorPage;

  beforeEach(() => {
    page = new WebAppMonitorPage();
  });

  it('should display welcome message', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('Welcome to app!!');
  });
});
