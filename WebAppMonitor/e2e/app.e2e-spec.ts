import { WebAppMonitorPage } from './app.po';

describe('web-app-monitor App', () => {
  let page: WebAppMonitorPage;

  beforeEach(() => {
    page = new WebAppMonitorPage();
  });

  it('should display welcome message', async () => {
    page.navigateTo();
    expect(await page.getParagraphText()).toEqual('Welcome to app!!');
  });
});
